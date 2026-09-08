// ASEZH — Chinese display overlay for Amplify Shader Editor.
// Keys and generated shader identifiers stay in their original English form.

using System;
using UnityEditor;
using UnityEngine;

namespace AmplifyShaderEditor
{
	public static class ASELocale
	{
		public const string TableCategory = "category";
		public const string TableNodeTitle = "node_title";
		public const string TableOptionLabel = "option_label";
		public const string TableOptionValue = "option_value";
		public const string TablePanel = "panel";

		const string LanguagePrefsKey = "ASE.Locale.UseChinese";
		static readonly string[] DisplayLookupOrder =
		{
			TableOptionLabel, TableCategory, TableNodeTitle, TablePanel, TableOptionValue
		};
		static readonly string[] ValueLookupOrder =
		{
			TableOptionValue, TableOptionLabel, TableNodeTitle, TableCategory, TablePanel
		};

		public static bool UseChinese
		{
			get { return EditorPrefs.GetBool( LanguagePrefsKey, true ); }
			set { EditorPrefs.SetBool( LanguagePrefsKey, value ); }
		}

		public static int EntryCount { get { return ASELocaleStore.EntryCount; } }

		public static void Reload()
		{
			ASELocaleStore.Reload();
			ASELocaleGuiAdapters.Reset();
		}

		public static string T( string key )
		{
			if( string.IsNullOrEmpty( key ) || !UseChinese )
				return key;
			string translated;
			return ASELocaleStore.TryOrdered( key, DisplayLookupOrder, out translated )
				? ASELocaleStore.ApplyTranslation( key, translated )
				: key;
		}

		public static string T( string key, string table )
		{
			if( string.IsNullOrEmpty( key ) || !UseChinese )
				return key;
			if( string.IsNullOrEmpty( table ) )
				return T( key );
			string translated;
			return ASELocaleStore.TryLookup( key, table, out translated )
				? ASELocaleStore.ApplyTranslation( key, translated )
				: key;
		}

		public static bool DrawLanguageToggle( Rect rect )
		{
			bool current = UseChinese;
			if( !ASELocaleGuiAdapters.DrawToggle( rect, current ) )
				return false;
			UseChinese = !current;
			return true;
		}

		public static string TranslateEnumName( string raw )
		{
			if( string.IsNullOrEmpty( raw ) )
				return raw;
			if( !UseChinese )
				return ObjectNames.NicifyVariableName( raw );
			string translated;
			if( ASELocaleStore.TryLookup( raw, TableOptionValue, out translated ) )
				return ASELocaleStore.ApplyTranslation( raw, translated );
			string nicified = ObjectNames.NicifyVariableName( raw );
			if( nicified != raw && ASELocaleStore.TryLookup( nicified, TableOptionValue, out translated ) )
				return translated;
			return T( raw );
		}

		public static Enum LayoutEnumPopup( GUIContent label, Enum selected, params GUILayoutOption[] options )
		{
			return ASELocaleGuiAdapters.LayoutEnumPopup( label, selected, TranslateEnumName, options );
		}

		public static Enum AreaEnumPopup( Rect position, Enum selected, GUIStyle style )
		{
			return ASELocaleGuiAdapters.AreaEnumPopup( position, selected, style, TranslateEnumName );
		}

		public static GUIContent GUI( GUIContent src )
		{
			return src == null ? null : new GUIContent( T( src.text ), src.image, src.tooltip );
		}

		/// <summary>Clones when translating. Never mutates the source array.</summary>
		public static string[] TranslateArray( string[] keys )
		{
			if( keys == null || !UseChinese )
				return keys;
			string[] result = new string[ keys.Length ];
			for( int i = 0; i < keys.Length; i++ )
			{
				string translated;
				result[ i ] = ASELocaleStore.TryOrdered( keys[ i ], ValueLookupOrder, out translated )
					? ASELocaleStore.ApplyTranslation( keys[ i ], translated )
					: keys[ i ];
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
				GUIContent source = keys[ i ];
				if( source == null )
				{
					result[ i ] = null;
					continue;
				}
				string translated;
				string text = ASELocaleStore.TryOrdered( source.text, ValueLookupOrder, out translated )
					? ASELocaleStore.ApplyTranslation( source.text, translated )
					: source.text;
				result[ i ] = new GUIContent( text, source.image, source.tooltip );
			}
			return result;
		}

		public static string TNodeListLabel( string name, string nameWithShortcut )
		{
			string translated = T( name, TableNodeTitle );
			if( string.IsNullOrEmpty( nameWithShortcut ) || nameWithShortcut == name )
				return translated;
			return nameWithShortcut.StartsWith( name, StringComparison.Ordinal )
				? translated + nameWithShortcut.Substring( name.Length )
				: translated;
		}

		public static bool MatchesSearch( string filter, string name, string category, string tags )
		{
			if( string.IsNullOrEmpty( filter ) || ( UseChinese && filter == T( "Search" ) ) )
				return true;
			string[] parts = filter.Trim().Split( new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries );
			for( int i = 0; i < parts.Length; i++ )
				if( !MatchesSearchPart( parts[ i ], name, category, tags ) )
					return false;
			return true;
		}

		static bool MatchesSearchPart( string part, string name, string category, string tags )
		{
			return ContainsIgnoreCase( name, part )
				|| ContainsIgnoreCase( category, part )
				|| ContainsIgnoreCase( tags, part )
				|| ContainsIgnoreCase( T( name, TableNodeTitle ), part )
				|| ContainsIgnoreCase( T( category, TableCategory ), part );
		}

		static bool ContainsIgnoreCase( string value, string part )
		{
			return !string.IsNullOrEmpty( value )
				&& value.IndexOf( part, StringComparison.CurrentCultureIgnoreCase ) >= 0;
		}

		public static string RunSelfTests()
		{
			bool previous = UseChinese;
			try
			{
				UseChinese = true;
				Reload();
				string failure = ValidateTranslations();
				if( !string.IsNullOrEmpty( failure ) ) return failure;
				string[] source = { "Add", "On", "Off" };
				string[] shown = TranslateArray( source );
				if( source[ 0 ] != "Add" || shown == source || shown[ 0 ] != "相加" || shown[ 1 ] != "开启" )
					return "TranslateArray clone/value contract failed";
				string[] platforms = { " Direct3D 11/12", " Vulkan", " PlayStation" };
				for( int i = 0; i < platforms.Length; i++ )
					if( T( platforms[ i ] ) != platforms[ i ] ) return "Platform translated: " + platforms[ i ];
				if( !MatchesSearch( "加法", "Add", "Math Operators", "add math" )
					|| !MatchesSearch( "Add", "Add", "Math Operators", "add math" )
					|| !MatchesSearch( "黑体", "Blackbody", "Functions", "blackbody" )
					|| !MatchesSearch( T( "Search" ), "Add", "Math Operators", "add math" ) )
					return "Search bilingual/fail-open contract failed";
				UseChinese = false;
				if( T( "Add" ) != "Add" || !object.ReferenceEquals( TranslateArray( source ), source ) )
					return "English mode must fail open";
				return null;
			}
			finally { UseChinese = previous; }
		}

		static string ValidateTranslations()
		{
			if( T( "Add" ) != "加法" ) return "T(Add) expected 加法, got " + T( "Add" );
			if( T( "Add", TableOptionValue ) != "相加" ) return "T(Add, option_value) expected 相加";
			if( T( "True" ) != "真" || T( "True", TableOptionValue ) != "是" ) return "True translations failed";
			if( T( "Blackbody", TableNodeTitle ) != "黑体" ) return "Blackbody translation failed";
			if( T( "Effect" ) != "效果" || T( "Distortion Amount" ) != "扭曲强度"
				|| T( "Use Distortion Mask" ) != "使用扭曲遮罩" ) return "Panel translations failed";
			return null;
		}
	}
}
