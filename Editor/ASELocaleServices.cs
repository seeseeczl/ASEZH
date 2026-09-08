using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace AmplifyShaderEditor
{
	[Serializable]
	internal sealed class ASEZHEntry
	{
		public string table;
		public string key;
		public string zh;
	}

	[Serializable]
	internal sealed class ASEZHFile
	{
		public ASEZHEntry[] entries;
	}

	internal static class ASELocaleStore
	{
		const string DictionaryAssetName = "ASEZHDictionary";
		static Dictionary<string, Dictionary<string, string>> s_tables;
		static bool s_loaded;
		static int s_entryCount;
		static int s_collisionCount;

		internal static int EntryCount
		{
			get { EnsureLoaded(); return s_entryCount; }
		}

		internal static void Reload()
		{
			s_loaded = false;
			s_tables = null;
			EnsureLoaded();
		}

		internal static bool TryLookup( string key, string table, out string zh )
		{
			zh = null;
			if( string.IsNullOrEmpty( key ) || string.IsNullOrEmpty( table ) )
				return false;
			EnsureLoaded();
			Dictionary<string, string> map;
			if( !s_tables.TryGetValue( table, out map ) )
				return false;
			if( map.TryGetValue( key, out zh ) )
				return true;
			int lead;
			int trail;
			SplitPadding( key, out lead, out trail );
			return ( lead != 0 || trail != key.Length )
				&& map.TryGetValue( key.Substring( lead, trail - lead ), out zh );
		}

		internal static bool TryOrdered( string key, string[] order, out string zh )
		{
			for( int i = 0; i < order.Length; i++ )
				if( TryLookup( key, order[ i ], out zh ) )
					return true;
			zh = null;
			return false;
		}

		internal static string ApplyTranslation( string key, string translated )
		{
			int lead;
			int trail;
			SplitPadding( key, out lead, out trail );
			return lead == 0 && trail == key.Length
				? translated
				: key.Substring( 0, lead ) + translated + key.Substring( trail );
		}

		static void EnsureLoaded()
		{
			if( s_loaded )
				return;
			s_loaded = true;
			s_tables = new Dictionary<string, Dictionary<string, string>>( StringComparer.Ordinal );
			s_entryCount = 0;
			s_collisionCount = 0;
			LoadJson( LocateDictionaryPath(), false );
			LoadJson( LocateUserOverlayPath(), true );
			string message = string.Format( "ASEZH: entries={0} collisions={1} tables={2}", s_entryCount, s_collisionCount, s_tables.Count );
			EditorApplication.delayCall += () => Debug.Log( message );
		}

		static void LoadJson( string path, bool overlayOnly )
		{
			if( string.IsNullOrEmpty( path ) || !File.Exists( path ) )
				return;
			try
			{
				ASEZHFile file = JsonUtility.FromJson<ASEZHFile>( File.ReadAllText( path ) );
				if( file == null || file.entries == null )
					return;
				for( int i = 0; i < file.entries.Length; i++ )
					AddEntry( file.entries[ i ], overlayOnly );
			}
			catch( Exception exception )
			{
				Debug.LogWarning( "ASEZH: failed to load " + path + ": " + exception.Message );
			}
		}

		static void AddEntry( ASEZHEntry entry, bool overlayOnly )
		{
			if( entry == null || string.IsNullOrEmpty( entry.table ) || string.IsNullOrEmpty( entry.key ) || string.IsNullOrEmpty( entry.zh ) )
				return;
			Dictionary<string, string> map;
			if( !s_tables.TryGetValue( entry.table, out map ) )
			{
				if( overlayOnly )
					return;
				map = new Dictionary<string, string>( StringComparer.Ordinal );
				s_tables[ entry.table ] = map;
			}
			string existing;
			if( overlayOnly && !map.ContainsKey( entry.key ) )
				return;
			if( map.TryGetValue( entry.key, out existing ) && existing != entry.zh && !overlayOnly )
			{
				s_collisionCount++;
				Debug.LogError( "ASEZH: same table+key different zh: " + entry.table + "/" + entry.key );
			}
			map[ entry.key ] = entry.zh;
			if( !overlayOnly )
				s_entryCount++;
		}

		static string LocateDictionaryPath()
		{
			string[] guids = AssetDatabase.FindAssets( DictionaryAssetName );
			for( int i = 0; i < guids.Length; i++ )
			{
				string path = AssetDatabase.GUIDToAssetPath( guids[ i ] );
				if( path.EndsWith( ".json", StringComparison.OrdinalIgnoreCase ) )
					return AseTargetResolver.ToAbsolute( path );
			}
			return null;
		}

		static string LocateUserOverlayPath()
		{
			string main = LocateDictionaryPath();
			return string.IsNullOrEmpty( main ) ? null : Path.Combine( Path.GetDirectoryName( main ), "ASEZHDictionary.user.json" );
		}

		static void SplitPadding( string key, out int lead, out int trail )
		{
			lead = 0;
			trail = key.Length;
			while( lead < key.Length && key[ lead ] == ' ' ) lead++;
			while( trail > lead && key[ trail - 1 ] == ' ' ) trail--;
		}
	}

	internal static class ASELocaleGuiAdapters
	{
		static GUIStyle s_toggleStyle;
		static readonly GUIContent LanguageOn = new GUIContent( "中文", "当前为中文。点击切换为原文。" );
		static readonly GUIContent LanguageOff = new GUIContent( "EN", "当前为原文。点击切换为中文。" );

		internal static void Reset() { s_toggleStyle = null; }

		internal static bool DrawToggle( Rect rect, bool current )
		{
			bool previousChanged = UnityEngine.GUI.changed;
			Color previousColor = UnityEngine.GUI.color;
			UnityEngine.GUI.color = current ? new Color( 0.55f, 0.82f, 1f, 1f ) : new Color( 1f, 1f, 1f, 0.72f );
			bool next = UnityEngine.GUI.Toggle( rect, current, current ? LanguageOn : LanguageOff, ToggleStyle );
			UnityEngine.GUI.color = previousColor;
			UnityEngine.GUI.changed = previousChanged;
			return next != current;
		}

		internal static Enum LayoutEnumPopup( GUIContent label, Enum selected, Func<string, string> translate, params GUILayoutOption[] options )
		{
			Type type = selected.GetType();
			if( Attribute.IsDefined( type, typeof( FlagsAttribute ) ) )
				return label == null ? EditorGUILayout.EnumPopup( selected, options ) : EditorGUILayout.EnumPopup( label, selected, options );
			Array values = Enum.GetValues( type );
			string[] names;
			int index;
			BuildEnumNames( values, selected, translate, out names, out index );
			int next = label == null ? EditorGUILayout.Popup( index, names, options ) : EditorGUILayout.Popup( label, index, names, options );
			return next < 0 || next >= values.Length ? selected : (Enum)values.GetValue( next );
		}

		internal static Enum AreaEnumPopup( Rect position, Enum selected, GUIStyle style, Func<string, string> translate )
		{
			Type type = selected.GetType();
			if( Attribute.IsDefined( type, typeof( FlagsAttribute ) ) )
				return style == null ? EditorGUI.EnumPopup( position, selected ) : EditorGUI.EnumPopup( position, selected, style );
			Array values = Enum.GetValues( type );
			string[] names;
			int index;
			BuildEnumNames( values, selected, translate, out names, out index );
			int next = style == null ? EditorGUI.Popup( position, index, names ) : EditorGUI.Popup( position, index, names, style );
			return next < 0 || next >= values.Length ? selected : (Enum)values.GetValue( next );
		}

		static void BuildEnumNames( Array values, Enum selected, Func<string, string> translate, out string[] names, out int index )
		{
			names = new string[ values.Length ];
			index = 0;
			for( int i = 0; i < values.Length; i++ )
			{
				object value = values.GetValue( i );
				names[ i ] = translate( value.ToString() );
				if( value.Equals( selected ) ) index = i;
			}
		}

		static GUIStyle ToggleStyle
		{
			get
			{
				if( s_toggleStyle == null )
				{
					s_toggleStyle = new GUIStyle( EditorStyles.miniButton );
					s_toggleStyle.alignment = TextAnchor.MiddleCenter;
					s_toggleStyle.fontSize = 11;
					s_toggleStyle.fontStyle = FontStyle.Bold;
					s_toggleStyle.padding = new RectOffset( 4, 4, 0, 0 );
				}
				return s_toggleStyle;
			}
		}
	}
}
