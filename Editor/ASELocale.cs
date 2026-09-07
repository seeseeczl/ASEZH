// ASEZH — Chinese display overlay for Amplify Shader Editor.
// KEY is always the original English string.
// Titles, labels, checkboxes and dropdown display text follow 中文/原文.
// Rendering platform names and shader identifiers stay English (not in the dictionary).

using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace AmplifyShaderEditor
{
	[Serializable]
	class ASEZHEntry
	{
		public string table;
		public string key;
		public string zh;
	}

	[Serializable]
	class ASEZHFile
	{
		public ASEZHEntry[] entries;
	}

	public static class ASELocale
	{
		public const string TableCategory = "category";
		public const string TableNodeTitle = "node_title";
		public const string TableOptionLabel = "option_label";
		public const string TableOptionValue = "option_value";
		public const string TablePanel = "panel";

		const string LanguagePrefsKey = "ASE.Locale.UseChinese";
		const string DictionaryAssetName = "ASEZHDictionary";

		static readonly string[] DisplayLookupOrder =
		{
			TableOptionLabel, TableCategory, TableNodeTitle, TablePanel, TableOptionValue
		};
		static readonly string[] ValueLookupOrder =
		{
			TableOptionValue, TableOptionLabel, TableNodeTitle, TableCategory, TablePanel
		};

		static Dictionary<string, Dictionary<string, string>> s_tables;
		static bool s_loaded;
		static int s_entryCount;
		static int s_collisionCount;
		static GUIStyle s_toggleStyle;
		static readonly GUIContent LanguageOnContent = new GUIContent( "中文", "当前为中文。点击切换为原文。" );
		static readonly GUIContent LanguageOffContent = new GUIContent( "EN", "当前为原文。点击切换为中文。" );

		public static bool UseChinese
		{
			get { return EditorPrefs.GetBool( LanguagePrefsKey, true ); }
			set { EditorPrefs.SetBool( LanguagePrefsKey, value ); }
		}

		public static int EntryCount
		{
			get
			{
				EnsureLoaded();
				return s_entryCount;
			}
		}

		public static void Reload()
		{
			s_loaded = false;
			s_tables = null;
			s_toggleStyle = null;
			EnsureLoaded();
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
			Debug.Log( string.Format( "ASEZH: entries={0} collisions={1} tables={2}", s_entryCount, s_collisionCount, s_tables.Count ) );
		}

		static string LocateDictionaryPath()
		{
			string[] guids = AssetDatabase.FindAssets( DictionaryAssetName );
			for( int i = 0; i < guids.Length; i++ )
			{
				string p = AssetDatabase.GUIDToAssetPath( guids[ i ] );
				if( p.EndsWith( ".json", StringComparison.OrdinalIgnoreCase ) )
					return ToAbsolute( p );
			}
			return null;
		}

		static string LocateUserOverlayPath()
		{
			string main = LocateDictionaryPath();
			if( string.IsNullOrEmpty( main ) )
				return null;
			return Path.Combine( Path.GetDirectoryName( main ), "ASEZHDictionary.user.json" );
		}

		static string ToAbsolute( string assetPath )
		{
			if( string.IsNullOrEmpty( assetPath ) )
				return assetPath;
			if( Path.IsPathRooted( assetPath ) )
				return assetPath;
			string root = Path.GetDirectoryName( Application.dataPath );
			return Path.GetFullPath( Path.Combine( root, assetPath ) );
		}

		static void LoadJson( string path, bool overlayOnly )
		{
			if( string.IsNullOrEmpty( path ) || !File.Exists( path ) )
				return;
			try
			{
				var file = JsonUtility.FromJson<ASEZHFile>( File.ReadAllText( path ) );
				if( file == null || file.entries == null )
					return;
				for( int i = 0; i < file.entries.Length; i++ )
				{
					var e = file.entries[ i ];
					if( e == null || string.IsNullOrEmpty( e.table ) || string.IsNullOrEmpty( e.key ) || string.IsNullOrEmpty( e.zh ) )
						continue;
					Dictionary<string, string> map;
					if( !s_tables.TryGetValue( e.table, out map ) )
					{
						if( overlayOnly )
							continue;
						map = new Dictionary<string, string>( StringComparer.Ordinal );
						s_tables[ e.table ] = map;
					}
					string existing;
					if( overlayOnly && !map.ContainsKey( e.key ) )
						continue;
					if( map.TryGetValue( e.key, out existing ) && existing != e.zh && !overlayOnly )
					{
						s_collisionCount++;
						Debug.LogError( "ASEZH: same table+key different zh: " + e.table + "/" + e.key );
					}
					map[ e.key ] = e.zh;
					if( !overlayOnly )
						s_entryCount++;
				}
			}
			catch( Exception ex )
			{
				Debug.LogWarning( "ASEZH: failed to load " + path + ": " + ex.Message );
			}
		}

		static void SplitPadding( string key, out int lead, out int trail )
		{
			lead = 0;
			trail = key.Length;
			while( lead < key.Length && key[ lead ] == ' ' )
				lead++;
			while( trail > lead && key[ trail - 1 ] == ' ' )
				trail--;
		}

		static bool TryTableLookup( string key, string table, out string zh )
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
			int lead, trail;
			SplitPadding( key, out lead, out trail );
			if( lead == 0 && trail == key.Length )
				return false;
			return map.TryGetValue( key.Substring( lead, trail - lead ), out zh );
		}

		static bool TryOrderedLookup( string key, string[] order, out string zh )
		{
			for( int i = 0; i < order.Length; i++ )
			{
				if( TryTableLookup( key, order[ i ], out zh ) )
					return true;
			}
			zh = null;
			return false;
		}

		static string ApplyZh( string key, string zh )
		{
			int lead, trail;
			SplitPadding( key, out lead, out trail );
			if( lead == 0 && trail == key.Length )
				return zh;
			return key.Substring( 0, lead ) + zh + key.Substring( trail );
		}

		public static string T( string key )
		{
			if( string.IsNullOrEmpty( key ) || !UseChinese )
				return key;
			string zh;
			if( !TryOrderedLookup( key, DisplayLookupOrder, out zh ) )
				return key;
			return ApplyZh( key, zh );
		}

		public static string T( string key, string table )
		{
			if( string.IsNullOrEmpty( key ) || !UseChinese )
				return key;
			if( string.IsNullOrEmpty( table ) )
				return T( key );
			string zh;
			if( !TryTableLookup( key, table, out zh ) )
				return key;
			return ApplyZh( key, zh );
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

		public static bool DrawLanguageToggle( Rect rect )
		{
			bool cur = UseChinese;
			bool prevChanged = UnityEngine.GUI.changed;
			Color old = UnityEngine.GUI.color;
			UnityEngine.GUI.color = cur ? new Color( 0.55f, 0.82f, 1f, 1f ) : new Color( 1f, 1f, 1f, 0.72f );
			bool next = UnityEngine.GUI.Toggle( rect, cur, cur ? LanguageOnContent : LanguageOffContent, ToggleStyle );
			UnityEngine.GUI.color = old;
			UnityEngine.GUI.changed = prevChanged;
			if( next == cur )
				return false;
			UseChinese = next;
			return true;
		}

		public static string TranslateEnumName( string raw )
		{
			if( string.IsNullOrEmpty( raw ) )
				return raw;
			if( !UseChinese )
				return ObjectNames.NicifyVariableName( raw );
			string zh;
			if( TryTableLookup( raw, TableOptionValue, out zh ) )
				return ApplyZh( raw, zh );
			string nicified = ObjectNames.NicifyVariableName( raw );
			if( nicified != raw && TryTableLookup( nicified, TableOptionValue, out zh ) )
				return zh;
			return T( raw );
		}

		public static Enum LayoutEnumPopup( GUIContent label, Enum selected, params GUILayoutOption[] options )
		{
			Type type = selected.GetType();
			if( Attribute.IsDefined( type, typeof( FlagsAttribute ) ) )
				return label == null ? EditorGUILayout.EnumPopup( selected, options ) : EditorGUILayout.EnumPopup( label, selected, options );

			Array values = Enum.GetValues( type );
			int count = values.Length;
			string[] names = new string[ count ];
			int index = 0;
			for( int i = 0; i < count; i++ )
			{
				object value = values.GetValue( i );
				names[ i ] = TranslateEnumName( value.ToString() );
				if( value.Equals( selected ) )
					index = i;
			}
			int next = label == null
				? EditorGUILayout.Popup( index, names, options )
				: EditorGUILayout.Popup( label, index, names, options );
			if( next < 0 || next >= count )
				return selected;
			return (Enum)values.GetValue( next );
		}

		public static Enum AreaEnumPopup( Rect position, Enum selected, GUIStyle style )
		{
			Type type = selected.GetType();
			if( Attribute.IsDefined( type, typeof( FlagsAttribute ) ) )
				return style == null ? EditorGUI.EnumPopup( position, selected ) : EditorGUI.EnumPopup( position, selected, style );

			Array values = Enum.GetValues( type );
			int count = values.Length;
			string[] names = new string[ count ];
			int index = 0;
			for( int i = 0; i < count; i++ )
			{
				object value = values.GetValue( i );
				names[ i ] = TranslateEnumName( value.ToString() );
				if( value.Equals( selected ) )
					index = i;
			}
			int next = style == null
				? EditorGUI.Popup( position, index, names )
				: EditorGUI.Popup( position, index, names, style );
			if( next < 0 || next >= count )
				return selected;
			return (Enum)values.GetValue( next );
		}

		public static GUIContent GUI( GUIContent src )
		{
			if( src == null )
				return src;
			return new GUIContent( T( src.text ), src.image, src.tooltip );
		}

		/// <summary>Clones when translating. Never mutates the source array.</summary>
		public static string[] TranslateArray( string[] keys )
		{
			if( keys == null || !UseChinese )
				return keys;
			string[] result = new string[ keys.Length ];
			for( int i = 0; i < keys.Length; i++ )
			{
				string key = keys[ i ];
				string zh;
				result[ i ] = TryOrderedLookup( key, ValueLookupOrder, out zh ) ? ApplyZh( key, zh ) : key;
			}
			return result;
		}

		public static GUIContent[] TranslateContents( GUIContent[] keys )
		{
			if( keys == null || !UseChinese )
				return keys;
			GUIContent[] result = new GUIContent[ keys.Length ];
			for( int i = 0; i < keys.Length; i++ )
			{
				GUIContent src = keys[ i ];
				if( src == null )
				{
					result[ i ] = null;
					continue;
				}
				string zh;
				string text = TryOrderedLookup( src.text, ValueLookupOrder, out zh ) ? ApplyZh( src.text, zh ) : src.text;
				result[ i ] = new GUIContent( text, src.image, src.tooltip );
			}
			return result;
		}

		public static string TNodeListLabel( string name, string nameWithShortcut )
		{
			string zh = T( name, TableNodeTitle );
			if( string.IsNullOrEmpty( nameWithShortcut ) || nameWithShortcut == name )
				return zh;
			if( nameWithShortcut.StartsWith( name, StringComparison.Ordinal ) )
				return zh + nameWithShortcut.Substring( name.Length );
			return zh;
		}

		public static bool MatchesSearch( string filter, string name, string category, string tags )
		{
			if( string.IsNullOrEmpty( filter ) )
				return true;
			string[] parts = filter.Trim().Split( new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries );
			for( int i = 0; i < parts.Length; i++ )
			{
				if( !MatchesSearchPart( parts[ i ], name, category, tags ) )
					return false;
			}
			return true;
		}

		static bool ContainsIgnoreCase( string haystack, string needle )
		{
			return !string.IsNullOrEmpty( haystack ) && haystack.IndexOf( needle, StringComparison.CurrentCultureIgnoreCase ) >= 0;
		}

		static bool MatchesSearchPart( string part, string name, string category, string tags )
		{
			if( ContainsIgnoreCase( name, part ) )
				return true;
			if( ContainsIgnoreCase( category, part ) )
				return true;
			if( ContainsIgnoreCase( tags, part ) )
				return true;
			if( ContainsIgnoreCase( T( name, TableNodeTitle ), part ) )
				return true;
			if( ContainsIgnoreCase( T( category, TableCategory ), part ) )
				return true;
			return false;
		}

		[MenuItem( "Window/ASEZH/Reload Dictionary", false, 2098 )]
		static void ReloadMenu()
		{
			Reload();
			string err = RunSelfTests();
			EditorUtility.DisplayDialog( "ASEZH",
				string.IsNullOrEmpty( err ) ? "Reloaded " + EntryCount + " keys. Self-tests passed." : err,
				"OK" );
		}

		[MenuItem( "Window/ASEZH/Run Locale Tests", false, 2099 )]
		static void RunLocaleTestsMenu()
		{
			EnsureLoaded();
			string err = RunSelfTests();
			EditorUtility.DisplayDialog( "ASEZH", string.IsNullOrEmpty( err ) ? "All locale tests passed." : err, "OK" );
		}

		public static string RunSelfTests()
		{
			bool prev = UseChinese;
			try
			{
				UseChinese = true;
				Reload();
				if( T( "Add" ) != "加法" )
					return "T(Add) expected 加法, got " + T( "Add" );
				if( T( "Add", TableOptionValue ) != "相加" )
					return "T(Add, option_value) expected 相加, got " + T( "Add", TableOptionValue );
				if( T( "True" ) != "真" )
					return "T(True) expected 真, got " + T( "True" );
				if( T( "True", TableOptionValue ) != "是" )
					return "T(True, option_value) expected 是, got " + T( "True", TableOptionValue );

				string[] src = { "Add", "On", "Off" };
				string[] shown = TranslateArray( src );
				if( src[ 0 ] != "Add" || src[ 1 ] != "On" )
					return "TranslateArray mutated source";
				if( shown == src )
					return "TranslateArray must clone in Chinese mode";
				if( shown[ 0 ] != "相加" || shown[ 1 ] != "开启" )
					return "TranslateArray values: " + string.Join( ",", shown );

				string[] platforms = { " Direct3D 11/12", " Vulkan", " PlayStation" };
				for( int i = 0; i < platforms.Length; i++ )
				{
					if( T( platforms[ i ] ) != platforms[ i ] )
						return "Platform translated: " + platforms[ i ];
				}

				if( !MatchesSearch( "加法", "Add", "Math Operators", "add math" ) )
					return "Search 加法 should hit Add";
				if( !MatchesSearch( "Add", "Add", "Math Operators", "add math" ) )
					return "Search Add should hit Add";
				if( T( "Blackbody", TableNodeTitle ) != "黑体" )
					return "T(Blackbody, node_title) expected 黑体, got " + T( "Blackbody", TableNodeTitle );
				if( !MatchesSearch( "黑体", "Blackbody", "Functions", "blackbody" ) )
					return "Search 黑体 should hit Blackbody";
				if( T( "Effect" ) != "效果" )
					return "T(Effect) expected 效果, got " + T( "Effect" );
				if( T( "Distortion Amount" ) != "扭曲强度" )
					return "T(Distortion Amount) expected 扭曲强度, got " + T( "Distortion Amount" );
				if( T( "Use Distortion Mask" ) != "使用扭曲遮罩" )
					return "T(Use Distortion Mask) expected 使用扭曲遮罩, got " + T( "Use Distortion Mask" );

				UseChinese = false;
				if( T( "Add" ) != "Add" )
					return "English mode must fail-open";
				if( !object.ReferenceEquals( TranslateArray( src ), src ) )
					return "English TranslateArray should return original array";
				return null;
			}
			finally
			{
				UseChinese = prev;
			}
		}
	}
}
