using System;
using System.Text.RegularExpressions;

namespace AmplifyShaderEditor
{
	internal static class ASEZHPaletteSearchEquivalence
	{
		const int SearchWindow = 2048;
		static readonly Regex LabelAssignment = new Regex(
			@"\bstring\s+(?<identifier>[A-Za-z_][A-Za-z0-9_]*)\s*=\s*ASELocale\.T\s*\(\s*m_searchFilterStr\s*\)\s*;" );

		internal static bool HasEquivalentLabelWidth( string text )
		{
			if( string.IsNullOrEmpty( text ) )
				return false;
			MatchCollection assignments = LabelAssignment.Matches( text );
			for( int i = 0; i < assignments.Count; i++ )
			{
				Match assignment = assignments[ i ];
				string identifier = assignment.Groups[ "identifier" ].Value;
				int remaining = Math.Min( SearchWindow, text.Length - assignment.Index - assignment.Length );
				string following = text.Substring( assignment.Index + assignment.Length, remaining );
				Match width = Regex.Match(
					following,
					@"\bm_searchLabelSize\s*=\s*GUI\.skin\.label\.CalcSize\s*\(\s*new\s+GUIContent\s*\(\s*"
						+ Regex.Escape( identifier ) + @"\s*\)\s*\)\.x\s*;" );
				if( !width.Success )
					continue;
				string beforeWidth = following.Substring( 0, width.Index );
				if( Regex.IsMatch( beforeWidth, @"\b" + Regex.Escape( identifier ) + @"\s*=" ) )
					continue;
				return true;
			}
			return false;
		}

		internal static bool IsEquivalentHook( string patchId, string text )
		{
			return ( patchId == "palette-search-label" || patchId == "palette-search-width" )
				&& HasEquivalentLabelWidth( text );
		}
	}
}
