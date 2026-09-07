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
					Marker = "ASELocale.MatchesSearch( m_searchFilter, allItems[ i ].Name, allItems[ i ].Category, allItems[ i ].Tags )",
					Find = "allItems[ i ].Name.IndexOf( m_searchFilter, StringComparison.InvariantCultureIgnoreCase ) >= 0 ||\n\t\t\t\t\tallItems[ i ].Category.IndexOf( m_searchFilter, StringComparison.InvariantCultureIgnoreCase ) >= 0",
					Replace = "ASELocale.MatchesSearch( m_searchFilter, allItems[ i ].Name, allItems[ i ].Category, allItems[ i ].Tags )"
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

		public static List<ASEZHPatchResult> RemoveAll()
		{
			var results = new List<ASEZHPatchResult>();
			var catalog = Catalog();
			for( int i = catalog.Count - 1; i >= 0; i-- )
				results.Add( Reverse( catalog[ i ] ) );
			results.Add( ReverseAseAssemblyReference() );
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

		static ASEZHPatchResult ReverseAseAssemblyReference()
		{
			var result = new ASEZHPatchResult { Id = "ase-asmdef-ref", File = "AmplifyShaderEditor.asmdef", Detail = "撤回 ASEZH.Editor 程序集引用" };
			string assetPath = FindAseAsmdef();
			if( string.IsNullOrEmpty( assetPath ) )
			{
				result.Status = "removed";
				result.Detail = "ASE 无独立 asmdef";
				return result;
			}
			result.File = assetPath;
			string abs = ToAbsolute( assetPath );
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

		static ASEZHPatchResult Evaluate( ASEZHPatch patch, bool apply )
		{
			if( patch.Id == "zwrite-labels" )
				return EnsureZWriteLabels( apply );
			if( patch.Id == "tools-language-toggle" )
				return EnsureLanguageToggle( apply );
			if( patch.Id == "palette-build-list" )
				return EnsurePaletteBuildList( apply );

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

		static ASEZHPatchResult Reverse( ASEZHPatch patch )
		{
			if( patch.Id == "zwrite-labels" )
				return RemoveZWriteLabels();
			if( patch.Id == "tools-language-toggle" )
				return RemoveLanguageToggle();
			if( patch.Id == "palette-build-list" )
				return RemovePaletteBuildList();

			var result = new ASEZHPatchResult { Id = patch.Id, File = patch.FileName, Detail = "撤回：" + patch.Description };
			string assetPath = FindFile( patch.FileName );
			if( string.IsNullOrEmpty( assetPath ) )
			{
				result.Status = "missing";
				result.Detail = "未找到 " + patch.FileName;
				return result;
			}
			result.File = assetPath;
			string abs = ToAbsolute( assetPath );
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

		static ASEZHPatchResult RemoveZWriteLabels()
		{
			var result = new ASEZHPatchResult { Id = "zwrite-labels", File = "ZBufferOpHelper.cs", Detail = "撤回 ZWriteModeLabels，Popup 改回 Values" };
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

		static ASEZHPatchResult RemoveLanguageToggle()
		{
			var result = new ASEZHPatchResult { Id = "tools-language-toggle", File = "ToolsWindow.cs", Detail = "撤回画布语言开关" };
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

		static ASEZHPatchResult RemovePaletteBuildList()
		{
			var result = new ASEZHPatchResult { Id = "palette-build-list", File = "PaletteParent.cs", Detail = "撤回 BuildFullList 的 MatchesSearch" };
			string assetPath = FindFile( "PaletteParent.cs" );
			if( string.IsNullOrEmpty( assetPath ) )
			{
				result.Status = "missing";
				result.Detail = "未找到 PaletteParent.cs";
				return result;
			}
			result.File = assetPath;
			string abs = ToAbsolute( assetPath );
			string text = File.ReadAllText( abs, Encoding.UTF8 );
			bool changed = false;
			if( BadPaletteSearchContinue.IsMatch( text ) )
			{
				text = BadPaletteSearchContinue.Replace( text, "" );
				changed = true;
			}
			var buildIf = new Regex(
				@"if\s*\(\s*ASELocale\.MatchesSearch\s*\(\s*m_searchFilter\s*,\s*allItems\[\s*i\s*\]\.Name\s*,\s*allItems\[\s*i\s*\]\.Category\s*,\s*allItems\[\s*i\s*\]\.Tags\s*\)\s*\)" );
			Match m = buildIf.Match( text );
			if( m.Success )
			{
				int after = m.Index + m.Length;
				while( after < text.Length && char.IsWhiteSpace( text[ after ] ) )
					after++;
				if( after < text.Length && text[ after ] == '{' )
				{
					string nl = text.IndexOf( "\r\n" ) >= 0 ? "\r\n" : "\n";
					string restored = "if( allItems[ i ].Name.IndexOf( m_searchFilter, StringComparison.InvariantCultureIgnoreCase ) >= 0 ||"
						+ nl + "\t\t\t\t\tallItems[ i ].Category.IndexOf( m_searchFilter, StringComparison.InvariantCultureIgnoreCase ) >= 0 )";
					text = text.Remove( m.Index, m.Length ).Insert( m.Index, restored );
					changed = true;
				}
			}
			if( !changed )
			{
				result.Status = "removed";
				result.Detail = "BuildFullList 无需撤回";
				return result;
			}
			File.WriteAllText( abs, text, new UTF8Encoding( false ) );
			result.Status = "removed";
			return result;
		}

		static readonly Regex BadPaletteSearchContinue = new Regex(
			@"if\s*\(\s*!ASELocale\.MatchesSearch\s*\(\s*m_searchFilter\s*,\s*allItems\[\s*i\s*\]\.Name\s*,\s*allItems\[\s*i\s*\]\.Category\s*,\s*allItems\[\s*i\s*\]\.Tags\s*\)\s*\)\s*continue\s*;\s*",
			RegexOptions.Multiline );

		static ASEZHPatchResult EnsurePaletteBuildList( bool apply )
		{
			var result = new ASEZHPatchResult { Id = "palette-build-list", File = "PaletteParent.cs", Detail = "Search 完整列表过滤支持中英文" };
			string assetPath = FindFile( "PaletteParent.cs" );
			if( string.IsNullOrEmpty( assetPath ) )
			{
				result.Status = "missing";
				result.Detail = "未找到 PaletteParent.cs";
				return result;
			}
			result.File = assetPath;
			string abs = ToAbsolute( assetPath );
			string text = File.ReadAllText( abs, Encoding.UTF8 );
			bool hasBadContinue = BadPaletteSearchContinue.IsMatch( text );
			bool hasIndexOfFilter = ContainsFlexible( text, "allItems[ i ].Name.IndexOf( m_searchFilter, StringComparison.InvariantCultureIgnoreCase )" );
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
			if( TryMatchFlexible( text, indexFind, out indexMatch ) )
			{
				string adapted = AdaptStyle( "ASELocale.MatchesSearch( m_searchFilter, allItems[ i ].Name, allItems[ i ].Category, allItems[ i ].Tags )", indexMatch.Value );
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
			hasIndexOfFilter = ContainsFlexible( text, "allItems[ i ].Name.IndexOf( m_searchFilter, StringComparison.InvariantCultureIgnoreCase )" );
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
			bool dictBroken = Regex.IsMatch( text, @"\{\s*ZTestMode\.Less\s*,\s*1\s*\}\s*;" );
			int labelsStart, labelsEnd, valuesStart, valuesEnd;
			bool hasCleanLabels = TryFindStringArrayField( text, "ZWriteModeLabels", out labelsStart, out labelsEnd )
				&& ArrayFieldIsClean( text, labelsStart, labelsEnd )
				&& ArrayFieldHasSemicolon( text, labelsEnd );
			bool popupLabels = Regex.IsMatch( text, @"EnumTypePopup\s*\(\s*ref\s+owner\s*,\s*ZWriteModeStr\s*,\s*ZWriteModeLabels" )
				|| Regex.IsMatch( text, @"EditorGUILayoutPopup\s*\(\s*ZWriteModeStr\s*,[\s\S]{0,80}?ZWriteModeLabels" );
			if( hasCleanLabels && popupLabels && !dictBroken )
			{
				result.Status = "applied";
				return result;
			}
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
			if( dictBroken )
				text = Regex.Replace( text, @"(\{\s*ZTestMode\.Less\s*,\s*1\s*\})\s*;", "$1 ," );
			StripOrphanZWriteLabelsAssignment( ref text );
			if( TryFindStringArrayField( text, "ZWriteModeLabels", out labelsStart, out labelsEnd ) && !ArrayFieldIsClean( text, labelsStart, labelsEnd ) )
				text = text.Remove( labelsStart, labelsEnd - labelsStart + 1 );
			if( !TryFindStringArrayField( text, "ZWriteModeLabels", out labelsStart, out labelsEnd ) || !ArrayFieldIsClean( text, labelsStart, labelsEnd ) )
			{
				if( !TryInsertZWriteLabelsArray( ref text ) )
				{
					result.Status = "mismatch";
					result.Detail = "无法从 ZWriteModeValues 复制 Labels 数组";
					return result;
				}
			}
			else if( !ArrayFieldHasSemicolon( text, labelsEnd ) )
				text = text.Insert( labelsEnd + 1, ";" );
			if( TryFindStringArrayField( text, "ZWriteModeValues", out valuesStart, out valuesEnd ) && !ArrayFieldHasSemicolon( text, valuesEnd ) )
				text = text.Insert( valuesEnd + 1, ";" );
			if( !popupLabels )
				TryRetargetZWritePopup( ref text );
			hasCleanLabels = TryFindStringArrayField( text, "ZWriteModeLabels", out labelsStart, out labelsEnd )
				&& ArrayFieldIsClean( text, labelsStart, labelsEnd )
				&& ArrayFieldHasSemicolon( text, labelsEnd );
			popupLabels = Regex.IsMatch( text, @"EnumTypePopup\s*\(\s*ref\s+owner\s*,\s*ZWriteModeStr\s*,\s*ZWriteModeLabels" )
				|| Regex.IsMatch( text, @"EditorGUILayoutPopup\s*\(\s*ZWriteModeStr\s*,[\s\S]{0,80}?ZWriteModeLabels" );
			if( !hasCleanLabels || !popupLabels )
			{
				result.Status = "mismatch";
				result.Detail = "ZWrite Labels 未能完整写入";
				return result;
			}
			File.WriteAllText( abs, text, new UTF8Encoding( false ) );
			result.Status = "patched";
			return result;
		}

		static bool TryFindStringArrayField( string text, string fieldName, out int start, out int end )
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

		static int MatchBalancedBrace( string text, int openIndex )
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

		static bool ArrayFieldIsClean( string text, int start, int end )
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

		static bool ArrayFieldHasSemicolon( string text, int end )
		{
			return end >= 0 && end < text.Length && text[ end ] == ';';
		}

		static void StripOrphanZWriteLabelsAssignment( ref string text )
		{
			text = Regex.Replace(
				text,
				@"(?:public\s+|private\s+|protected\s+)?(?:static\s+)?(?:readonly\s+)?string\s*\[\s*\]\s+ZWriteModeLabels\s*=\s*(?=(?:public|private|protected)\s)",
				"" );
		}

		static bool TryInsertZWriteLabelsArray( ref string text )
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
