using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace AmplifyShaderEditor
{
	internal static class ASEZHSpecialPatchTransforms
	{
		internal static ASEZHPatchResult RemoveZWriteLabels()
		{
			var result = new ASEZHPatchResult { Id = "zwrite-labels", File = "ZBufferOpHelper.cs", Detail = "撤回 ZWriteModeLabels，Popup 改回 Values" };
			string assetPath = ASEZHPatcher.FindFile( "ZBufferOpHelper.cs" );
			if( string.IsNullOrEmpty( assetPath ) )
			{
				result.Status = "missing";
				result.Detail = "未找到 ZBufferOpHelper.cs";
				return result;
			}
			result.File = assetPath;
			string abs = ASEZHPatcher.ToAbsolute( assetPath );
			string text = File.ReadAllText( abs, Encoding.UTF8 );
			bool changed = false;
			int labelsStart, labelsEnd;
			if( TryFindStringArrayField( text, "ZWriteModeLabels", out labelsStart, out labelsEnd ) && ArrayFieldIsClean( text, labelsStart, labelsEnd ) )
			{
				int from = labelsStart;
				while( from > 0 && ( text[ from - 1 ] == ' ' || text[ from - 1 ] == '\t' ) )
					from--;
				if( from > 0 && text[ from - 1 ] == '\n' )
					from--;
				if( from > 0 && text[ from - 1 ] == '\r' )
					from--;
				text = text.Remove( from, labelsEnd - from + 1 );
				changed = true;
			}
			string nextPopup = Regex.Replace( text, @"(EnumTypePopup\s*\(\s*ref\s+owner\s*,\s*ZWriteModeStr\s*,\s*)ZWriteModeLabels", "$1ZWriteModeValues" );
			nextPopup = Regex.Replace( nextPopup, @"(EditorGUILayoutPopup\s*\(\s*ZWriteModeStr\s*,[^,]+,\s*)ZWriteModeLabels", "$1ZWriteModeValues" );
			if( nextPopup != text )
			{
				text = nextPopup;
				changed = true;
			}
			if( !changed )
			{
				result.Status = "removed";
				result.Detail = "没有可撤回的 ZWrite Labels";
				return result;
			}
			File.WriteAllText( abs, text, new UTF8Encoding( false ) );
			result.Status = "removed";
			return result;
		}

		internal static ASEZHPatchResult RemovePaletteBuildList()
		{
			var result = new ASEZHPatchResult { Id = "palette-build-list", File = "PaletteParent.cs", Detail = "撤回 BuildFullList 的 MatchesSearch" };
			string assetPath = ASEZHPatcher.FindFile( "PaletteParent.cs" );
			if( string.IsNullOrEmpty( assetPath ) )
			{
				result.Status = "missing";
				result.Detail = "未找到 PaletteParent.cs";
				return result;
			}
			result.File = assetPath;
			string abs = ASEZHPatcher.ToAbsolute( assetPath );
			string text = File.ReadAllText( abs, Encoding.UTF8 );
			string next;
			bool changed = TryRemovePaletteBuildListText( text, out next );
			text = next;
			if( !changed )
			{
				result.Status = PaletteBuildIf.IsMatch( text )
					? "mismatch"
					: "removed";
				result.Detail = result.Status == "mismatch" ? "仍有 MatchesSearch 钩子但无法安全撤回" : "BuildFullList 无需撤回";
				return result;
			}
			if( PaletteBuildIf.IsMatch( text ) )
			{
				result.Status = "mismatch";
				result.Detail = "撤回后仍有 MatchesSearch 钩子";
				return result;
			}
			File.WriteAllText( abs, text, new UTF8Encoding( false ) );
			result.Status = "removed";
			return result;
		}

		internal static bool IsAlternatePaletteItemOverload( string patchId, string text )
		{
			if( patchId == "palette-node-item" )
				return ASEZHPatchTransforms.ContainsFlexible( text, "EditorGUI.Toggle( thisRect, current.Value.Contents[ i ].ItemUIContent, false, EditorStyles.label );" );
			if( patchId == "palette-node-item-content" )
				return ASEZHPatchTransforms.ContainsFlexible( text, "EditorGUI.Toggle( thisRect, current.Value.Contents[ i ].ItemUIContent.text, false, EditorStyles.label );" );
			return false;
		}

		internal static bool TryRemovePaletteBuildListText( string input, out string output )
		{
			string text = input;
			bool changed = false;
			if( BadPaletteSearchContinue.IsMatch( text ) )
			{
				text = BadPaletteSearchContinue.Replace( text, "" );
				changed = true;
			}
			Match m = PaletteBuildIf.Match( text );
			if( m.Success )
			{
				string nl = text.IndexOf( "\r\n", StringComparison.Ordinal ) >= 0 ? "\r\n" : "\n";
				string indent = ASEZHPatchTransforms.LineIndent( text, m.Index );
				string continuation = indent.IndexOf( '\t' ) >= 0 ? indent + "\t" : indent + "    ";
				string restored = "if( allItems[ i ].Name.IndexOf( m_searchFilter, StringComparison.InvariantCultureIgnoreCase ) >= 0 ||"
					+ nl + continuation + "allItems[ i ].Category.IndexOf( m_searchFilter, StringComparison.InvariantCultureIgnoreCase ) >= 0 )";
				text = text.Remove( m.Index, m.Length ).Insert( m.Index, restored );
				changed = true;
			}
			output = text;
			return changed;
		}

		internal static readonly Regex BadPaletteSearchContinue = new Regex(
			@"if\s*\(\s*!ASELocale\.MatchesSearch\s*\(\s*m_searchFilter\s*,\s*allItems\[\s*i\s*\]\.Name\s*,\s*allItems\[\s*i\s*\]\.Category\s*,\s*allItems\[\s*i\s*\]\.Tags\s*\)\s*\)\s*continue\s*;\s*",
			RegexOptions.Multiline );

		internal static readonly Regex PaletteBuildIf = new Regex(
			@"if\s*\(\s*ASELocale\.MatchesSearch\s*\(\s*m_searchFilter\s*,\s*allItems\[\s*i\s*\]\.Name\s*,\s*allItems\[\s*i\s*\]\.Category\s*,\s*allItems\[\s*i\s*\]\.Tags\s*\)\s*\)" );

		internal static ASEZHPatchResult EnsurePaletteBuildList( bool apply )
		{
			var result = new ASEZHPatchResult { Id = "palette-build-list", File = "PaletteParent.cs", Detail = "Search 完整列表过滤支持中英文" };
			string assetPath = ASEZHPatcher.FindFile( "PaletteParent.cs" );
			if( string.IsNullOrEmpty( assetPath ) )
			{
				result.Status = "missing";
				result.Detail = "未找到 PaletteParent.cs";
				return result;
			}
			result.File = assetPath;
			string abs = ASEZHPatcher.ToAbsolute( assetPath );
			string text = File.ReadAllText( abs, Encoding.UTF8 );
			bool hasBadContinue = BadPaletteSearchContinue.IsMatch( text );
			bool hasIndexOfFilter = ASEZHPatchTransforms.ContainsFlexible( text, "allItems[ i ].Name.IndexOf( m_searchFilter, StringComparison.InvariantCultureIgnoreCase )" );
			if( !hasBadContinue && !hasIndexOfFilter )
			{
				result.Status = "applied";
				return result;
			}
			if( !apply )
			{
				result.Status = "ready";
				result.Detail += hasBadContinue ? "（将去掉会清空空搜索列表的 continue）" : "（将把 BuildFullList 的 IndexOf 换成 MatchesSearch）";
				return result;
			}
			if( hasBadContinue )
				text = BadPaletteSearchContinue.Replace( text, "" );
			Match indexMatch;
			const string indexFind = "allItems[ i ].Name.IndexOf( m_searchFilter, StringComparison.InvariantCultureIgnoreCase ) >= 0 ||\n\t\t\t\t\tallItems[ i ].Category.IndexOf( m_searchFilter, StringComparison.InvariantCultureIgnoreCase ) >= 0";
			if( ASEZHPatchTransforms.TryMatchFlexible( text, indexFind, out indexMatch ) )
			{
				string adapted = ASEZHPatchTransforms.AdaptStyle( "ASELocale.MatchesSearch( m_searchFilter, allItems[ i ].Name, allItems[ i ].Category, allItems[ i ].Tags )", indexMatch.Value );
				text = text.Remove( indexMatch.Index, indexMatch.Length ).Insert( indexMatch.Index, adapted );
			}
			else
			{
				text = Regex.Replace(
					text,
					@"allItems\[\s*i\s*\]\.Name\.IndexOf\s*\(\s*m_searchFilter\s*,\s*StringComparison\.InvariantCultureIgnoreCase\s*\)\s*>=\s*0\s*\|\|\s*allItems\[\s*i\s*\]\.Category\.IndexOf\s*\(\s*m_searchFilter\s*,\s*StringComparison\.InvariantCultureIgnoreCase\s*\)\s*>=\s*0",
					"ASELocale.MatchesSearch( m_searchFilter, allItems[ i ].Name, allItems[ i ].Category, allItems[ i ].Tags )" );
			}
			hasBadContinue = BadPaletteSearchContinue.IsMatch( text );
			hasIndexOfFilter = ASEZHPatchTransforms.ContainsFlexible( text, "allItems[ i ].Name.IndexOf( m_searchFilter, StringComparison.InvariantCultureIgnoreCase )" );
			if( hasBadContinue || hasIndexOfFilter )
			{
				result.Status = "mismatch";
				result.Detail = "无法去掉错误的 MatchesSearch continue，或 BuildFullList 的 IndexOf 未换成 MatchesSearch";
				return result;
			}
			File.WriteAllText( abs, text, new UTF8Encoding( false ) );
			result.Status = "patched";
			return result;
		}

		internal static ASEZHPatchResult EnsureZWriteLabels( bool apply )
		{
			var result = new ASEZHPatchResult { Id = "zwrite-labels", File = "ZBufferOpHelper.cs", Detail = "ZWrite 显示数组与 Shader 值数组拆开（补 Labels 定义）" };
			string assetPath = ASEZHPatcher.FindFile( "ZBufferOpHelper.cs" );
			if( string.IsNullOrEmpty( assetPath ) )
			{
				result.Status = "missing";
				result.Detail = "未找到 ZBufferOpHelper.cs";
				return result;
			}
			result.File = assetPath;
			string abs = ASEZHPatcher.ToAbsolute( assetPath );
			string text = File.ReadAllText( abs, Encoding.UTF8 );
			bool dictBroken = Regex.IsMatch( text, @"\{\s*ZTestMode\.Less\s*,\s*1\s*\}\s*;" );
			int labelsStart, labelsEnd;
			bool hasCleanLabels = HasCleanZWriteLabels( text, out labelsStart, out labelsEnd );
			bool popupLabels = UsesZWriteLabels( text );
			if( hasCleanLabels && popupLabels && !dictBroken )
			{
				result.Status = "applied";
				return result;
			}
			int valuesStart, valuesEnd;
			bool canInsert = TryFindStringArrayField( text, "ZWriteModeValues", out valuesStart, out valuesEnd )
				&& ArrayFieldIsClean( text, valuesStart, valuesEnd );
			bool canRetarget = Regex.IsMatch( text, @"EnumTypePopup\s*\(\s*ref\s+owner\s*,\s*ZWriteModeStr\s*,\s*ZWriteModeValues" )
				|| Regex.IsMatch( text, @"EditorGUILayoutPopup\s*\(\s*ZWriteModeStr\s*,[\s\S]{0,80}?ZWriteModeValues" );
			bool canFix = canInsert || ( !popupLabels && canRetarget ) || dictBroken
				|| ( TryFindStringArrayField( text, "ZWriteModeLabels", out labelsStart, out labelsEnd ) && !ArrayFieldIsClean( text, labelsStart, labelsEnd ) );
			if( !canFix )
			{
				result.Status = "mismatch";
				result.Detail = "未找到完整的 ZWriteModeValues 数组，或 ZBufferOpHelper.cs 已被写坏，请从 ASE 备份恢复后再接入";
				return result;
			}
			if( !apply )
			{
				result.Status = "ready";
				result.Detail += dictBroken ? "（将修复 ZTestModeDict 被误插入的分号）" : "（将按括号配对写入 Labels 数组）";
				return result;
			}
			if( !TryRepairZWriteLabels( ref text, dictBroken, popupLabels ) )
			{
				result.Status = "mismatch";
				result.Detail = "ZWrite Labels 未能完整写入或无法从 Values 数组复制";
				return result;
			}
			File.WriteAllText( abs, text, new UTF8Encoding( false ) );
			result.Status = "patched";
			return result;
		}

		static bool HasCleanZWriteLabels( string text, out int start, out int end )
		{
			return TryFindStringArrayField( text, "ZWriteModeLabels", out start, out end )
				&& ArrayFieldIsClean( text, start, end )
				&& ArrayFieldHasSemicolon( text, end );
		}

		static bool UsesZWriteLabels( string text )
		{
			return Regex.IsMatch( text, @"EnumTypePopup\s*\(\s*ref\s+owner\s*,\s*ZWriteModeStr\s*,\s*ZWriteModeLabels" )
				|| Regex.IsMatch( text, @"EditorGUILayoutPopup\s*\(\s*ZWriteModeStr\s*,[\s\S]{0,80}?ZWriteModeLabels" );
		}

		static bool TryRepairZWriteLabels( ref string text, bool dictBroken, bool popupLabels )
		{
			if( dictBroken )
				text = Regex.Replace( text, @"(\{\s*ZTestMode\.Less\s*,\s*1\s*\})\s*;", "$1 ," );
			StripOrphanZWriteLabelsAssignment( ref text );
			int labelsStart, labelsEnd;
			if( TryFindStringArrayField( text, "ZWriteModeLabels", out labelsStart, out labelsEnd ) && !ArrayFieldIsClean( text, labelsStart, labelsEnd ) )
				text = text.Remove( labelsStart, labelsEnd - labelsStart + 1 );
			if( !TryFindStringArrayField( text, "ZWriteModeLabels", out labelsStart, out labelsEnd ) || !ArrayFieldIsClean( text, labelsStart, labelsEnd ) )
			{
				if( !TryInsertZWriteLabelsArray( ref text ) )
					return false;
			}
			else if( !ArrayFieldHasSemicolon( text, labelsEnd ) )
				text = text.Insert( labelsEnd + 1, ";" );
			int valuesStart, valuesEnd;
			if( TryFindStringArrayField( text, "ZWriteModeValues", out valuesStart, out valuesEnd ) && !ArrayFieldHasSemicolon( text, valuesEnd ) )
				text = text.Insert( valuesEnd + 1, ";" );
			if( !popupLabels )
				TryRetargetZWritePopup( ref text );
			return HasCleanZWriteLabels( text, out labelsStart, out labelsEnd ) && UsesZWriteLabels( text );
		}

		internal static bool TryFindStringArrayField( string text, string fieldName, out int start, out int end )
		{
			start = -1;
			end = -1;
			Match m = Regex.Match( text, @"(?:public\s+|private\s+|protected\s+)?(?:static\s+)?(?:readonly\s+)?string\s*\[\s*\]\s+" + Regex.Escape( fieldName ) + @"\s*=" );
			if( !m.Success )
				return false;
			int brace = m.Index + m.Length;
			while( brace < text.Length && char.IsWhiteSpace( text[ brace ] ) )
				brace++;
			if( brace >= text.Length || text[ brace ] != '{' )
				return false;
			int close = MatchBalancedBrace( text, brace );
			if( close < 0 )
				return false;
			start = m.Index;
			end = close;
			int j = close + 1;
			while( j < text.Length && char.IsWhiteSpace( text[ j ] ) )
				j++;
			if( j < text.Length && text[ j ] == ';' )
				end = j;
			return true;
		}

		internal static int MatchBalancedBrace( string text, int openIndex )
		{
			int depth = 0;
			bool inStr = false;
			for( int i = openIndex; i < text.Length; i++ )
			{
				char c = text[ i ];
				if( inStr )
				{
					if( c == '\\' && i + 1 < text.Length )
					{
						i++;
						continue;
					}
					if( c == '"' )
						inStr = false;
					continue;
				}
				if( c == '"' )
				{
					inStr = true;
					continue;
				}
				if( c == '{' )
					depth++;
				else if( c == '}' )
				{
					depth--;
					if( depth == 0 )
						return i;
				}
			}
			return -1;
		}

		internal static bool ArrayFieldIsClean( string text, int start, int end )
		{
			if( start < 0 || end < start || end >= text.Length )
				return false;
			string span = text.Substring( start, end - start + 1 );
			if( span.IndexOf( "Dictionary", StringComparison.Ordinal ) >= 0 )
				return false;
			if( span.IndexOf( "ZTestMode", StringComparison.Ordinal ) >= 0 )
				return false;
			return end - start < 800;
		}

		internal static bool ArrayFieldHasSemicolon( string text, int end )
		{
			return end >= 0 && end < text.Length && text[ end ] == ';';
		}

		internal static void StripOrphanZWriteLabelsAssignment( ref string text )
		{
			text = Regex.Replace(
				text,
				@"(?:public\s+|private\s+|protected\s+)?(?:static\s+)?(?:readonly\s+)?string\s*\[\s*\]\s+ZWriteModeLabels\s*=\s*(?=(?:public|private|protected)\s)",
				"" );
		}

		internal static bool TryInsertZWriteLabelsArray( ref string text )
		{
			int valuesStart, valuesEnd;
			if( !TryFindStringArrayField( text, "ZWriteModeValues", out valuesStart, out valuesEnd ) )
				return false;
			if( !ArrayFieldIsClean( text, valuesStart, valuesEnd ) )
				return false;
			if( !ArrayFieldHasSemicolon( text, valuesEnd ) )
			{
				text = text.Insert( valuesEnd + 1, ";" );
				valuesEnd++;
			}
			int labelsStart, labelsEnd;
			if( TryFindStringArrayField( text, "ZWriteModeLabels", out labelsStart, out labelsEnd ) && ArrayFieldIsClean( text, labelsStart, labelsEnd ) )
				return true;
			string labels = text.Substring( valuesStart, valuesEnd - valuesStart + 1 ).Replace( "ZWriteModeValues", "ZWriteModeLabels" );
			string nl = text.IndexOf( "\r\n" ) >= 0 ? "\r\n" : "\n";
			text = text.Insert( valuesEnd + 1, nl + nl + labels );
			return true;
		}

		internal static void TryRetargetZWritePopup( ref string text )
		{
			text = Regex.Replace( text, @"(EnumTypePopup\s*\(\s*ref\s+owner\s*,\s*ZWriteModeStr\s*,\s*)ZWriteModeValues", "$1ZWriteModeLabels" );
			text = Regex.Replace( text, @"(EditorGUILayoutPopup\s*\(\s*ZWriteModeStr\s*,[^,]+,\s*)ZWriteModeValues", "$1ZWriteModeLabels" );
		}
	}
}
