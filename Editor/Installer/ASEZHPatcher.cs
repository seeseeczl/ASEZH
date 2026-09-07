using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace AmplifyShaderEditor
{
	public class ASEZHPatchResult
	{
		public string Id;
		public string File;
		public string Status;
		public string Detail;
	}

	[Serializable]
	public class ASEZHPatch
	{
		public string Id;
		public string Description;
		public string FileName;
		public string Find;
		public string Replace;
		public string Marker;
	}

	public static class ASEZHPatcher
	{
		public static List<ASEZHPatch> Catalog()
		{
			return new List<ASEZHPatch>
			{
				new ASEZHPatch
				{
					Id = "undo-popup-string",
					Description = "UndoParentNode Popup：翻译 label 与下拉显示数组",
					FileName = "UndoParentNode.cs",
					Marker = "ASELocale.TranslateArray( displayedOptions )",
					Find = "public int EditorGUILayoutPopup( string label, int selectedIndex, string[] displayedOptions, params GUILayoutOption[] options )\n\t\t{\n\t\t\tint newValue = EditorGUILayout.Popup( label, selectedIndex, displayedOptions, options );",
					Replace = "public int EditorGUILayoutPopup( string label, int selectedIndex, string[] displayedOptions, params GUILayoutOption[] options )\n\t\t{\n\t\t\tlabel = ASELocale.T( label );\n\t\t\tint newValue = EditorGUILayout.Popup( label, selectedIndex, ASELocale.TranslateArray( displayedOptions ), options );"
				},
				new ASEZHPatch
				{
					Id = "undo-popup-no-label",
					Description = "UndoParentNode Popup(无 label)",
					FileName = "UndoParentNode.cs",
					Marker = "EditorGUILayout.Popup( selectedIndex, ASELocale.TranslateArray( displayedOptions )",
					Find = "int newValue = EditorGUILayout.Popup( selectedIndex, displayedOptions, options );",
					Replace = "int newValue = EditorGUILayout.Popup( selectedIndex, ASELocale.TranslateArray( displayedOptions ), options );"
				},
				new ASEZHPatch
				{
					Id = "undo-enum-string",
					Description = "UndoParentNode EnumPopup(string)",
					FileName = "UndoParentNode.cs",
					Marker = "ASELocale.LayoutEnumPopup( new GUIContent( label )",
					Find = "public Enum EditorGUILayoutEnumPopup( string label, Enum selected, params GUILayoutOption[] options )\n\t\t{\n\t\t\tEnum newValue = EditorGUILayout.EnumPopup( label, selected, options );",
					Replace = "public Enum EditorGUILayoutEnumPopup( string label, Enum selected, params GUILayoutOption[] options )\n\t\t{\n\t\t\tlabel = ASELocale.T( label );\n\t\t\tEnum newValue = ASELocale.LayoutEnumPopup( new GUIContent( label ), selected, options );"
				},
				new ASEZHPatch
				{
					Id = "undo-enum-bare",
					Description = "UndoParentNode EnumPopup(Enum)",
					FileName = "UndoParentNode.cs",
					Marker = "ASELocale.LayoutEnumPopup( null, selected",
					Find = "public Enum EditorGUILayoutEnumPopup( Enum selected, params GUILayoutOption[] options )\n\t\t{\n\t\t\tEnum newValue = EditorGUILayout.EnumPopup( selected, options );",
					Replace = "public Enum EditorGUILayoutEnumPopup( Enum selected, params GUILayoutOption[] options )\n\t\t{\n\t\t\tEnum newValue = ASELocale.LayoutEnumPopup( null, selected, options );"
				},
				new ASEZHPatch
				{
					Id = "undo-toggle-string",
					Description = "UndoParentNode Toggle(string)",
					FileName = "UndoParentNode.cs",
					Marker = "label = ASELocale.T( label );\n\t\t\tbool newValue = EditorGUILayout.Toggle( label, value, options );",
					Find = "public bool EditorGUILayoutToggle( string label, bool value, params GUILayoutOption[] options )\n\t\t{\n\t\t\tbool newValue = EditorGUILayout.Toggle( label, value, options );",
					Replace = "public bool EditorGUILayoutToggle( string label, bool value, params GUILayoutOption[] options )\n\t\t{\n\t\t\tlabel = ASELocale.T( label );\n\t\t\tbool newValue = EditorGUILayout.Toggle( label, value, options );"
				},
				new ASEZHPatch
				{
					Id = "undo-toggleleft-string",
					Description = "UndoParentNode ToggleLeft(string)",
					FileName = "UndoParentNode.cs",
					Marker = "label = ASELocale.T( label );\n\t\t\tbool newValue = EditorGUILayout.ToggleLeft( label, value, options );",
					Find = "public bool EditorGUILayoutToggleLeft( string label, bool value, params GUILayoutOption[] options )\n\t\t{\n\t\t\tbool newValue = EditorGUILayout.ToggleLeft( label, value, options );",
					Replace = "public bool EditorGUILayoutToggleLeft( string label, bool value, params GUILayoutOption[] options )\n\t\t{\n\t\t\tlabel = ASELocale.T( label );\n\t\t\tbool newValue = EditorGUILayout.ToggleLeft( label, value, options );"
				},
				new ASEZHPatch
				{
					Id = "tools-language-toggle",
					Description = "工具栏语言开关（源码图标右侧）",
					FileName = "ToolsWindow.cs",
					Marker = "ASELocale.DrawLanguageToggle",
					Find = "m_openSourceCodeButton.Draw( TabX + m_transformedArea.x + m_openSourceCodeButton.ButtonSpacing, TabY );\n\n\t\t\tGUI.color = bufferedColor;",
					Replace = "m_openSourceCodeButton.Draw( TabX + m_transformedArea.x + m_openSourceCodeButton.ButtonSpacing, TabY );\n\n\t\t\tGUI.color = bufferedColor;\n\t\t\tconst float sourceIconW = 24f;\n\t\t\tconst float langW = 46f;\n\t\t\tconst float langH = 21f;\n\t\t\tRect languageRect = new Rect(\n\t\t\t\tTabX + m_transformedArea.x + m_openSourceCodeButton.ButtonSpacing + sourceIconW + 8f,\n\t\t\t\tTabY,\n\t\t\t\tlangW,\n\t\t\t\tlangH );\n\t\t\tif( ASELocale.DrawLanguageToggle( languageRect ) )\n\t\t\t\tm_parentWindow.RequestRepaint();"
				},
				new ASEZHPatch
				{
					Id = "nodeutils-group-title",
					Description = "折叠分组标题",
					FileName = "NodeUtils.cs",
					Marker = "ASELocale.T( sectionName )",
					Find = "GUILayout.Label( sectionName, UIUtils.MenuItemToggleStyle );",
					Replace = "GUILayout.Label( ASELocale.T( sectionName ), UIUtils.MenuItemToggleStyle );"
				},
				new ASEZHPatch
				{
					Id = "parentnode-title",
					Description = "画布节点标题",
					FileName = "ParentNode.cs",
					Marker = "ASELocale.T( m_content.text, ASELocale.TableNodeTitle )",
					Find = "GUI.Label( titlePos, m_content, UIUtils.GetCustomStyle( CustomStyle.NodeTitle ) );",
					Replace = "GUI.Label( titlePos, new GUIContent( ASELocale.T( m_content.text, ASELocale.TableNodeTitle ), m_content.image, m_content.tooltip ), UIUtils.GetCustomStyle( CustomStyle.NodeTitle ) );"
				},
				new ASEZHPatch
				{
					Id = "zwrite-labels",
					Description = "ZWrite 显示数组与 Shader 值数组拆开（补 Labels 定义）",
					FileName = "ZBufferOpHelper.cs",
					Marker = "string[] ZWriteModeLabels",
					Find = "m_zWriteMode.EnumTypePopup( ref owner, ZWriteModeStr, ZWriteModeValues );",
					Replace = "m_zWriteMode.EnumTypePopup( ref owner, ZWriteModeStr, ZWriteModeLabels );"
				},
				new ASEZHPatch
				{
					Id = "palette-search-label",
					Description = "Search 窗口搜索框标签",
					FileName = "PaletteParent.cs",
					Marker = "ASELocale.T( m_searchFilterStr )",
					Find = "m_searchFilter = EditorGUILayout.TextField( m_searchFilterStr, m_searchFilter );",
					Replace = "m_searchFilter = EditorGUILayout.TextField( ASELocale.T( m_searchFilterStr ), m_searchFilter );"
				},
				new ASEZHPatch
				{
					Id = "palette-search-width",
					Description = "Search 标签宽度按译文计算",
					FileName = "PaletteParent.cs",
					Marker = "new GUIContent( ASELocale.T( m_searchFilterStr ) )",
					Find = "m_searchLabelSize = GUI.skin.label.CalcSize( new GUIContent( m_searchFilterStr ) ).x;",
					Replace = "m_searchLabelSize = GUI.skin.label.CalcSize( new GUIContent( ASELocale.T( m_searchFilterStr ) ) ).x;"
				},
				new ASEZHPatch
				{
					Id = "palette-category",
					Description = "Search 分类折叠标题（Camera And Screen 等）",
					FileName = "PaletteParent.cs",
					Marker = "ASELocale.T( current.Key, ASELocale.TableCategory )",
					Find = "bool visible = GUILayout.Toggle( current.Value.Visible, current.Key, m_foldoutStyle );",
					Replace = "bool visible = GUILayout.Toggle( current.Value.Visible, ASELocale.T( current.Key, ASELocale.TableCategory ), m_foldoutStyle );"
				},
				new ASEZHPatch
				{
					Id = "palette-node-item",
					Description = "Search 节点行显示名",
					FileName = "PaletteParent.cs",
					Marker = "ASELocale.TNodeListLabel(",
					Find = "EditorGUI.Toggle( thisRect, current.Value.Contents[ i ].ItemUIContent.text, false, EditorStyles.label );",
					Replace = "EditorGUI.Toggle( thisRect, ASELocale.TNodeListLabel( current.Value.Contents[ i ].Name, current.Value.Contents[ i ].ItemUIContent.text ), false, EditorStyles.label );"
				},
				new ASEZHPatch
				{
					Id = "palette-node-item-content",
					Description = "Search 节点行显示名（GUIContent 重载）",
					FileName = "PaletteParent.cs",
					Marker = "ASELocale.TNodeListLabel(",
					Find = "EditorGUI.Toggle( thisRect, current.Value.Contents[ i ].ItemUIContent, false, EditorStyles.label );",
					Replace = "EditorGUI.Toggle( thisRect, new GUIContent( ASELocale.TNodeListLabel( current.Value.Contents[ i ].Name, current.Value.Contents[ i ].ItemUIContent.text ), current.Value.Contents[ i ].ItemUIContent.image, current.Value.Contents[ i ].ItemUIContent.tooltip ), false, EditorStyles.label );"
				},
				new ASEZHPatch
				{
					Id = "palette-search-filter",
					Description = "Search 过滤支持中英文",
					FileName = "PaletteParent.cs",
					Marker = "bool localeHit = ASELocale.MatchesSearch",
					Find = "if( searchList.Length == matchesFound )",
					Replace = "bool localeHit = ASELocale.MatchesSearch( m_searchFilter, allItems[ i ].Name, allItems[ i ].Category, allItems[ i ].Tags );\n\t\t\t\t\t\t\t\tif( localeHit || searchList.Length == matchesFound )"
				},
				new ASEZHPatch
				{
					Id = "palette-build-list",
					Description = "Search 完整列表过滤支持中英文",
					FileName = "PaletteParent.cs",
					Marker = "if( !ASELocale.MatchesSearch( m_searchFilter, allItems[ i ].Name",
					Find = "m_currentItems.Add( allItems[ i ] );\n\t\t\t\t\tif( !m_currentCategories.ContainsKey( allItems[ i ].Category ) )",
					Replace = "if( !ASELocale.MatchesSearch( m_searchFilter, allItems[ i ].Name, allItems[ i ].Category, allItems[ i ].Tags ) )\n\t\t\t\t\t\tcontinue;\n\t\t\t\t\tm_currentItems.Add( allItems[ i ] );\n\t\t\t\t\tif( !m_currentCategories.ContainsKey( allItems[ i ].Category ) )"
				}
			};
		}

		public static string FindAseRoot()
		{
			string[] guids = AssetDatabase.FindAssets( "UndoParentNode t:MonoScript" );
			for( int i = 0; i <  guids.Length; i++ )
			{
				string path = AssetDatabase.GUIDToAssetPath(  guids[ i ] );
				if( path.Replace( '\\', '/' ).EndsWith( "/UndoParentNode.cs" ) )
					return Path.GetDirectoryName( Path.GetDirectoryName( path ) );
			}
			return null;
		}

		public static string FindFile( string fileName )
		{
			string[] guids = AssetDatabase.FindAssets( Path.GetFileNameWithoutExtension( fileName ) + " t:MonoScript" );
			for( int i = 0; i <  guids.Length; i++ )
			{
				string path = AssetDatabase.GUIDToAssetPath(  guids[ i ] );
				if( path.Replace( '\\', '/' ).EndsWith( "/" + fileName ) )
					return path;
			}
			return null;
		}

		public static List<ASEZHPatchResult> Scan()
		{
			var results = new List<ASEZHPatchResult>();
			results.Add( EnsureAseAssemblyReference( false ) );
			var catalog = Catalog();
			for( int i = 0; i < catalog.Count; i++ )
				results.Add( Evaluate( catalog[ i ], false ) );
			return results;
		}

		public static List<ASEZHPatchResult> ApplyAll()
		{
			var results = new List<ASEZHPatchResult>();
			results.Add( EnsureAseAssemblyReference( true ) );
			var catalog = Catalog();
			for( int i = 0; i < catalog.Count; i++ )
				results.Add( Evaluate( catalog[ i ], true ) );
			AssetDatabase.Refresh();
			return results;
		}

		const string LocaleAssemblyName = "ASEZH.Editor";

		static string FindAseAsmdef()
		{
			string[] guids = AssetDatabase.FindAssets( "AmplifyShaderEditor t:AssemblyDefinitionAsset" );
			for( int i = 0; i < guids.Length; i++ )
			{
				string path = AssetDatabase.GUIDToAssetPath( guids[ i ] );
				if( path.Replace( '\\', '/' ).EndsWith( "/AmplifyShaderEditor.asmdef" ) )
					return path;
			}
			return null;
		}

		public static ASEZHPatchResult EnsureAseAssemblyReference( bool apply )
		{
			var result = new ASEZHPatchResult { Id = "ase-asmdef-ref", File = "AmplifyShaderEditor.asmdef" };
			string assetPath = FindAseAsmdef();
			if( string.IsNullOrEmpty( assetPath ) )
			{
				result.Status = "applied";
				result.Detail = "ASE 无独立 asmdef（预定义程序集可自动引用 ASEZH.Editor）";
				return result;
			}
			result.File = assetPath;
			string abs = ToAbsolute( assetPath );
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

		static ASEZHPatchResult Evaluate( ASEZHPatch patch, bool apply )
		{
			if( patch.Id == "zwrite-labels" )
				return EnsureZWriteLabels( apply );
			if( patch.Id == "tools-language-toggle" )
				return EnsureLanguageToggle( apply );

			var result = new ASEZHPatchResult { Id = patch.Id, File = patch.FileName };
			string assetPath = FindFile( patch.FileName );
			if( string.IsNullOrEmpty( assetPath ) )
			{
				result.Status = "missing";
				result.Detail = "未找到 " + patch.FileName + "（ASE 版本结构可能不同）";
				return result;
			}
			result.File = assetPath;
			string abs = ToAbsolute( assetPath );
			string text = File.ReadAllText( abs, Encoding.UTF8 );
			if( !string.IsNullOrEmpty( patch.Marker ) && ContainsFlexible( text, patch.Marker ) )
			{
				result.Status = "applied";
				result.Detail = patch.Description;
				return result;
			}
			Match findMatch;
			if( string.IsNullOrEmpty( patch.Find ) || !TryMatchFlexible( text, patch.Find, out findMatch ) )
			{
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

		static ASEZHPatchResult EnsureZWriteLabels( bool apply )
		{
			var result = new ASEZHPatchResult { Id = "zwrite-labels", File = "ZBufferOpHelper.cs", Detail = "ZWrite 显示数组与 Shader 值数组拆开（补 Labels 定义）" };
			string assetPath = FindFile( "ZBufferOpHelper.cs" );
			if( string.IsNullOrEmpty( assetPath ) )
			{
				result.Status = "missing";
				result.Detail = "未找到 ZBufferOpHelper.cs";
				return result;
			}
			result.File = assetPath;
			string abs = ToAbsolute( assetPath );
			string text = File.ReadAllText( abs, Encoding.UTF8 );
			bool hasArray = Regex.IsMatch( text, @"string\s*\[\s*\]\s*ZWriteModeLabels\s*=\s*\{.*?\}\s*;", RegexOptions.Singleline );
			bool popupLabels = Regex.IsMatch( text, @"EnumTypePopup\s*\(\s*ref\s+owner\s*,\s*ZWriteModeStr\s*,\s*ZWriteModeLabels" )
				|| Regex.IsMatch( text, @"EditorGUILayoutPopup\s*\(\s*ZWriteModeStr\s*,[\s\S]{0,80}?ZWriteModeLabels" );
			bool needsSemicolonRepair = Regex.IsMatch( text, @"string\s*\[\s*\]\s*ZWriteMode(?:Values|Labels)\s*=\s*\{.*?\}(?!\s*;)", RegexOptions.Singleline );
			if( hasArray && popupLabels && !needsSemicolonRepair )
			{
				result.Status = "applied";
				return result;
			}
			bool canInsert = Regex.IsMatch( text, @"string\s*\[\s*\]\s*ZWriteModeValues\s*=" );
			bool canRetarget = Regex.IsMatch( text, @"EnumTypePopup\s*\(\s*ref\s+owner\s*,\s*ZWriteModeStr\s*,\s*ZWriteModeValues" )
				|| Regex.IsMatch( text, @"EditorGUILayoutPopup\s*\(\s*ZWriteModeStr\s*,[\s\S]{0,80}?ZWriteModeValues" );
			bool canFix = ( !hasArray && canInsert ) || ( !popupLabels && canRetarget ) || needsSemicolonRepair;
			if( !canFix )
			{
				result.Status = "mismatch";
				result.Detail = "未找到 ZWriteModeValues 定义或 Popup 用法，需按 docs/hook-sites.md 手工接入";
				return result;
			}
			if( !apply )
			{
				result.Status = "ready";
				if( needsSemicolonRepair )
					result.Detail += "（将补数组末尾分号）";
				else if( !hasArray )
					result.Detail += "（将补数组定义）";
				return result;
			}
			if( needsSemicolonRepair )
				TryRepairZWriteArraySemicolons( ref text );
			if( !hasArray && !TryInsertZWriteLabelsArray( ref text ) )
			{
				result.Status = "mismatch";
				result.Detail = "无法从 ZWriteModeValues 复制 Labels 数组";
				return result;
			}
			if( !popupLabels )
				TryRetargetZWritePopup( ref text );
			hasArray = Regex.IsMatch( text, @"string\s*\[\s*\]\s*ZWriteModeLabels" );
			popupLabels = Regex.IsMatch( text, @"EnumTypePopup\s*\(\s*ref\s+owner\s*,\s*ZWriteModeStr\s*,\s*ZWriteModeLabels" )
				|| Regex.IsMatch( text, @"EditorGUILayoutPopup\s*\(\s*ZWriteModeStr\s*,[\s\S]{0,80}?ZWriteModeLabels" );
			if( !hasArray || !popupLabels )
			{
				result.Status = "mismatch";
				result.Detail = "ZWrite Labels 未能完整写入";
				return result;
			}
			File.WriteAllText( abs, text, new UTF8Encoding( false ) );
			result.Status = "patched";
			return result;
		}

		static bool TryInsertZWriteLabelsArray( ref string text )
		{
			if( Regex.IsMatch( text, @"string\s*\[\s*\]\s*ZWriteModeLabels\s*=\s*\{.*?\}\s*;", RegexOptions.Singleline ) )
				return true;
			var rx = new Regex( @"((?:public\s+)?static\s+readonly\s+string\s*\[\s*\]\s*ZWriteModeValues\s*=\s*\{.*?\}\s*;)", RegexOptions.Singleline );
			Match m = rx.Match( text );
			if( !m.Success )
				return false;
			string labels = m.Value.Replace( "ZWriteModeValues", "ZWriteModeLabels" );
			if( !labels.TrimEnd().EndsWith( ";" ) )
				labels += ";";
			string nl = text.IndexOf( "\r\n" ) >= 0 ? "\r\n" : "\n";
			text = text.Insert( m.Index + m.Length, nl + nl + labels );
			return true;
		}

		static void TryRepairZWriteArraySemicolons( ref string text )
		{
			text = Regex.Replace(
				text,
				@"(string\s*\[\s*\]\s*ZWriteMode(?:Values|Labels)\s*=\s*\{.*?\})(\s*)(?!;)",
				"$1;$2",
				RegexOptions.Singleline );
		}

		static void TryRetargetZWritePopup( ref string text )
		{
			text = Regex.Replace( text, @"(EnumTypePopup\s*\(\s*ref\s+owner\s*,\s*ZWriteModeStr\s*,\s*)ZWriteModeValues", "$1ZWriteModeLabels" );
			text = Regex.Replace( text, @"(EditorGUILayoutPopup\s*\(\s*ZWriteModeStr\s*,[^,]+,\s*)ZWriteModeValues", "$1ZWriteModeLabels" );
		}

		static ASEZHPatchResult EnsureLanguageToggle( bool apply )
		{
			var result = new ASEZHPatchResult { Id = "tools-language-toggle", File = "ToolsWindow.cs", Detail = "工具栏语言开关（源码图标右侧）" };
			string assetPath = FindFile( "ToolsWindow.cs" );
			if( string.IsNullOrEmpty( assetPath ) )
			{
				result.Status = "missing";
				result.Detail = "未找到 ToolsWindow.cs";
				return result;
			}
			result.File = assetPath;
			string abs = ToAbsolute( assetPath );
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

		static bool ContainsFlexible( string text, string needle )
		{
			if( string.IsNullOrEmpty( needle ) )
				return false;
			if( text.IndexOf( needle, StringComparison.Ordinal ) >= 0 )
				return true;
			Match unused;
			return TryMatchFlexible( text, needle, out unused );
		}

		static bool TryMatchFlexible( string text, string find, out Match match )
		{
			match = Match.Empty;
			if( string.IsNullOrEmpty( find ) )
				return false;
			match = Regex.Match( text, ToFlexiblePattern( find ) );
			return match.Success;
		}

		static string ToFlexiblePattern( string find )
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

		static string AdaptStyle( string catalogReplace, string matched )
		{
			string r = catalogReplace.Replace( "\r\n", "\n" );
			bool crlf = matched.IndexOf( "\r\n" ) >= 0;
			if( matched.IndexOf( '\t' ) < 0 )
				r = r.Replace( "\t", "    " );
			if( crlf )
				r = r.Replace( "\n", "\r\n" );
			return r;
		}

		static string LineIndent( string text, int index )
		{
			int start = index;
			while( start > 0 && text[ start - 1 ] != '\n' )
				start--;
			int i = start;
			while( i < index && ( text[ i ] == ' ' || text[ i ] == '\t' ) )
				i++;
			return text.Substring( start, i - start );
		}

		static string ToAbsolute( string assetPath )
		{
			string root = Path.GetDirectoryName( Application.dataPath );
			return Path.GetFullPath( Path.Combine( root, assetPath ) );
		}
	}
}
