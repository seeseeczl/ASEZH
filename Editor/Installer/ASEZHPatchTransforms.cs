using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace AmplifyShaderEditor
{
	internal static class ASEZHPatchTransforms
	{
		const string LocaleAssemblyName = "ASEZH.Editor";
		internal static ASEZHPatchResult EnsureAseAssemblyReference( bool apply )
		{
			var result = new ASEZHPatchResult { Id = "ase-asmdef-ref", File = "AmplifyShaderEditor.asmdef" };
			string assetPath = ASEZHPatcher.FindAseAsmdef();
			if( string.IsNullOrEmpty( assetPath ) )
			{
				result.Status = "applied";
				result.Detail = "ASE 无独立 asmdef（预定义程序集可自动引用 ASEZH.Editor）";
				return result;
			}
			result.File = assetPath;
			string abs = ASEZHPatcher.ToAbsolute( assetPath );
			string text = File.ReadAllText( abs, Encoding.UTF8 );
			if( text.Contains( "\"" + LocaleAssemblyName + "\"" ) )
			{
				result.Status = "applied";
				result.Detail = "AmplifyShaderEditor 已引用 ASEZH.Editor";
				return result;
			}
			if( !apply )
			{
				result.Status = "ready";
				result.Detail = "ASE 使用独立程序集，必须引用 ASEZH.Editor，否则 ASELocale 找不到";
				return result;
			}
			if( text.Contains( "\"references\": []" ) )
				text = text.Replace( "\"references\": []", "\"references\": [ \"" + LocaleAssemblyName + "\" ]" );
			else if( text.Contains( "\"references\":[]" ) )
				text = text.Replace( "\"references\":[]", "\"references\":[\"" + LocaleAssemblyName + "\"]" );
			else
			{
				result.Status = "mismatch";
				result.Detail = "无法自动写入 references，请在 AmplifyShaderEditor.asmdef 中手动加入 ASEZH.Editor";
				return result;
			}
			File.WriteAllText( abs, text, new UTF8Encoding( false ) );
			result.Status = "patched";
			result.Detail = "已让 AmplifyShaderEditor 引用 ASEZH.Editor";
			return result;
		}

		internal static ASEZHPatchResult ReverseAseAssemblyReference()
		{
			var result = new ASEZHPatchResult { Id = "ase-asmdef-ref", File = "AmplifyShaderEditor.asmdef", Detail = "撤回 ASEZH.Editor 程序集引用" };
			string assetPath = ASEZHPatcher.FindAseAsmdef();
			if( string.IsNullOrEmpty( assetPath ) )
			{
				result.Status = "removed";
				result.Detail = "ASE 无独立 asmdef";
				return result;
			}
			result.File = assetPath;
			string abs = ASEZHPatcher.ToAbsolute( assetPath );
			string text = File.ReadAllText( abs, Encoding.UTF8 );
			if( !text.Contains( "\"" + LocaleAssemblyName + "\"" ) )
			{
				result.Status = "removed";
				result.Detail = "未引用 ASEZH.Editor";
				return result;
			}
			string next = text.Replace( "\"references\": [ \"" + LocaleAssemblyName + "\" ]", "\"references\": []" )
				.Replace( "\"references\":[\"" + LocaleAssemblyName + "\"]", "\"references\":[]" );
			if( next == text )
			{
				result.Status = "mismatch";
				result.Detail = "asmdef 还有其它 references，请手工删掉 ASEZH.Editor";
				return result;
			}
			File.WriteAllText( abs, next, new UTF8Encoding( false ) );
			result.Status = "removed";
			return result;
		}

		internal static ASEZHPatchResult Evaluate( ASEZHPatch patch, bool apply )
		{
			if( patch.Id == "zwrite-labels" )
				return ASEZHSpecialPatchTransforms.EnsureZWriteLabels( apply );
			if( patch.Id == "tools-language-toggle" )
				return EnsureLanguageToggle( apply );
			if( patch.Id == "palette-build-list" )
				return ASEZHSpecialPatchTransforms.EnsurePaletteBuildList( apply );

			var result = new ASEZHPatchResult { Id = patch.Id, File = patch.FileName };
			string assetPath = ASEZHPatcher.FindFile( patch.FileName );
			if( string.IsNullOrEmpty( assetPath ) )
			{
				result.Status = "missing";
				result.Detail = "未找到 " + patch.FileName + "（ASE 版本结构可能不同）";
				return result;
			}
			result.File = assetPath;
			string abs = ASEZHPatcher.ToAbsolute( assetPath );
			string text = File.ReadAllText( abs, Encoding.UTF8 );
			if( patch.Id == "palette-search-width" && ASEZHPaletteSearchEquivalence.HasEquivalentLabelWidth( text ) )
			{
				result.Status = "applied";
				result.Detail = patch.Description + "（已识别等价局部变量写法）";
				return result;
			}
			if( !string.IsNullOrEmpty( patch.Marker ) && ContainsFlexible( text, patch.Marker ) )
			{
				result.Status = "applied";
				result.Detail = patch.Description;
				return result;
			}
			Match findMatch;
			if( string.IsNullOrEmpty( patch.Find ) || !TryMatchFlexible( text, patch.Find, out findMatch ) )
			{
				if( ASEZHSpecialPatchTransforms.IsAlternatePaletteItemOverload( patch.Id, text ) )
				{
					result.Status = "not-applicable";
					result.Detail = "当前 ASE 使用另一个 Search 节点行 Toggle 重载";
					return result;
				}
				result.Status = "mismatch";
				result.Detail = "锚点未命中，需按 docs/adapt-ase-version.md 手工接入：" + patch.Description;
				return result;
			}
			if( !apply )
			{
				result.Status = "ready";
				result.Detail = patch.Description;
				return result;
			}
			string adapted = AdaptStyle( patch.Replace, findMatch.Value );
			string next = text.Remove( findMatch.Index, findMatch.Length ).Insert( findMatch.Index, adapted );
			if( next == text )
			{
				result.Status = "mismatch";
				result.Detail = "替换未改变文件";
				return result;
			}
			File.WriteAllText( abs, next, new UTF8Encoding( false ) );
			result.Status = "patched";
			result.Detail = patch.Description;
			return result;
		}

		internal static ASEZHPatchResult Reverse( ASEZHPatch patch )
		{
			if( patch.Id == "zwrite-labels" )
				return ASEZHSpecialPatchTransforms.RemoveZWriteLabels();
			if( patch.Id == "tools-language-toggle" )
				return RemoveLanguageToggle();
			if( patch.Id == "palette-build-list" )
				return ASEZHSpecialPatchTransforms.RemovePaletteBuildList();

			var result = new ASEZHPatchResult { Id = patch.Id, File = patch.FileName, Detail = "撤回：" + patch.Description };
			string assetPath = ASEZHPatcher.FindFile( patch.FileName );
			if( string.IsNullOrEmpty( assetPath ) )
			{
				result.Status = "missing";
				result.Detail = "未找到 " + patch.FileName;
				return result;
			}
			result.File = assetPath;
			string abs = ASEZHPatcher.ToAbsolute( assetPath );
			string text = File.ReadAllText( abs, Encoding.UTF8 );
			Match replaceMatch;
			if( !string.IsNullOrEmpty( patch.Replace ) && TryMatchFlexible( text, patch.Replace, out replaceMatch ) )
			{
				string adapted = AdaptStyle( patch.Find, replaceMatch.Value );
				string next = text.Remove( replaceMatch.Index, replaceMatch.Length ).Insert( replaceMatch.Index, adapted );
				if( next != text )
				{
					File.WriteAllText( abs, next, new UTF8Encoding( false ) );
					result.Status = "removed";
					return result;
				}
			}
			if( !string.IsNullOrEmpty( patch.Find ) && ContainsFlexible( text, patch.Find )
				&& ( string.IsNullOrEmpty( patch.Marker ) || !ContainsFlexible( text, patch.Marker ) ) )
			{
				result.Status = "removed";
				result.Detail = "源码已是接入前片段";
				return result;
			}
			if( ASEZHPaletteSearchEquivalence.IsEquivalentHook( patch.Id, text ) )
			{
				result.Status = "removed";
				result.Detail = "检测到非内联等价写法，保留源码；仅安装回执可恢复接入前内容";
				return result;
			}
			if( patch.Id == "palette-node-item" || patch.Id == "palette-node-item-content" )
			{
				result.Status = "removed";
				result.Detail = "此 Toggle 重载未接入或已撤回";
				return result;
			}
			if( !string.IsNullOrEmpty( patch.Marker ) && ContainsFlexible( text, patch.Marker ) )
			{
				result.Status = "mismatch";
				result.Detail = "仍有钩子但无法按锚点撤回，需手工还原 " + patch.FileName;
				return result;
			}
			result.Status = "removed";
			result.Detail = "未找到对应钩子";
			return result;
		}

		internal static ASEZHPatchResult RemoveLanguageToggle()
		{
			var result = new ASEZHPatchResult { Id = "tools-language-toggle", File = "ToolsWindow.cs", Detail = "撤回画布语言开关" };
			string assetPath = ASEZHPatcher.FindFile( "ToolsWindow.cs" );
			if( string.IsNullOrEmpty( assetPath ) )
			{
				result.Status = "missing";
				result.Detail = "未找到 ToolsWindow.cs";
				return result;
			}
			result.File = assetPath;
			string abs = ASEZHPatcher.ToAbsolute( assetPath );
			string text = File.ReadAllText( abs, Encoding.UTF8 );
			if( !ContainsFlexible( text, "ASELocale.DrawLanguageToggle" ) )
			{
				result.Status = "removed";
				result.Detail = "未接入语言开关";
				return result;
			}
			string next = Regex.Replace(
				text,
				@"\s*const float sourceIconW = 24f;[\s\S]*?if\s*\(\s*ASELocale\.DrawLanguageToggle\s*\(\s*languageRect\s*\)\s*\)\s*m_parentWindow\.RequestRepaint\s*\(\s*\)\s*;",
				"" );
			if( next == text )
			{
				result.Status = "mismatch";
				result.Detail = "找到 DrawLanguageToggle 但无法按插入块撤回";
				return result;
			}
			File.WriteAllText( abs, next, new UTF8Encoding( false ) );
			result.Status = "removed";
			return result;
		}

		internal static ASEZHPatchResult EnsureLanguageToggle( bool apply )
		{
			var result = new ASEZHPatchResult { Id = "tools-language-toggle", File = "ToolsWindow.cs", Detail = "工具栏语言开关（源码图标右侧）" };
			string assetPath = ASEZHPatcher.FindFile( "ToolsWindow.cs" );
			if( string.IsNullOrEmpty( assetPath ) )
			{
				result.Status = "missing";
				result.Detail = "未找到 ToolsWindow.cs";
				return result;
			}
			result.File = assetPath;
			string abs = ASEZHPatcher.ToAbsolute( assetPath );
			string text = File.ReadAllText( abs, Encoding.UTF8 );
			if( ContainsFlexible( text, "ASELocale.DrawLanguageToggle" ) )
			{
				result.Status = "applied";
				return result;
			}
			var drawRx = new Regex( @"m_openSourceCodeButton\.Draw\(\s*TabX\s*\+\s*m_transformedArea\.x\s*\+\s*m_openSourceCodeButton\.ButtonSpacing\s*,\s*TabY\s*\)\s*;" );
			Match draw = drawRx.Match( text );
			if( !draw.Success )
			{
				result.Status = "mismatch";
				result.Detail = "未找到源码图标 Draw 调用，需按 docs/hook-sites.md 手工接入";
				return result;
			}
			if( !apply )
			{
				result.Status = "ready";
				return result;
			}
			string after = text.Substring( draw.Index + draw.Length );
			Match color = Regex.Match( after, @"\A\s*GUI\.color\s*=\s*bufferedColor\s*;" );
			int insertAt = draw.Index + draw.Length + ( color.Success ? color.Length : 0 );
			string indent = LineIndent( text, draw.Index );
			string nl = text.IndexOf( "\r\n" ) >= 0 ? "\r\n" : "\n";
			string tab = indent.IndexOf( '\t' ) >= 0 ? "\t" : "    ";
			string block = "";
			if( !color.Success && text.IndexOf( "bufferedColor" ) >= 0 )
				block += nl + indent + "GUI.color = bufferedColor;";
			block += nl + indent + "const float sourceIconW = 24f;"
				+ nl + indent + "const float langW = 46f;"
				+ nl + indent + "const float langH = 21f;"
				+ nl + indent + "Rect languageRect = new Rect("
				+ nl + indent + tab + "TabX + m_transformedArea.x + m_openSourceCodeButton.ButtonSpacing + sourceIconW + 8f,"
				+ nl + indent + tab + "TabY,"
				+ nl + indent + tab + "langW,"
				+ nl + indent + tab + "langH );"
				+ nl + indent + "if( ASELocale.DrawLanguageToggle( languageRect ) )"
				+ nl + indent + tab + "m_parentWindow.RequestRepaint();";
			text = text.Insert( insertAt, block );
			File.WriteAllText( abs, text, new UTF8Encoding( false ) );
			result.Status = "patched";
			return result;
		}

		internal static bool ContainsFlexible( string text, string needle )
		{
			if( string.IsNullOrEmpty( needle ) )
				return false;
			if( text.IndexOf( needle, StringComparison.Ordinal ) >= 0 )
				return true;
			Match unused;
			return TryMatchFlexible( text, needle, out unused );
		}

		internal static bool TryMatchFlexible( string text, string find, out Match match )
		{
			match = Match.Empty;
			if( string.IsNullOrEmpty( find ) )
				return false;
			match = Regex.Match( text, ToFlexiblePattern( find ) );
			return match.Success;
		}

		internal static string ToFlexiblePattern( string find )
		{
			var sb = new StringBuilder();
			bool inWs = false;
			for( int i = 0; i < find.Length; i++ )
			{
				char c = find[ i ];
				if( char.IsWhiteSpace( c ) )
				{
					if( !inWs )
					{
						sb.Append( @"\s+" );
						inWs = true;
					}
				}
				else
				{
					inWs = false;
					sb.Append( Regex.Escape( c.ToString() ) );
				}
			}
			return sb.ToString();
		}

		internal static string AdaptStyle( string catalogReplace, string matched )
		{
			string r = catalogReplace.Replace( "\r\n", "\n" );
			bool crlf = matched.IndexOf( "\r\n" ) >= 0;
			if( matched.IndexOf( '\t' ) < 0 )
				r = r.Replace( "\t", "    " );
			if( crlf )
				r = r.Replace( "\n", "\r\n" );
			return r;
		}

		internal static string LineIndent( string text, int index )
		{
			int start = index;
			while( start > 0 && text[ start - 1 ] != '\n' )
				start--;
			int i = start;
			while( i < index && ( text[ i ] == ' ' || text[ i ] == '\t' ) )
				i++;
			return text.Substring( start, i - start );
		}
	}
}
