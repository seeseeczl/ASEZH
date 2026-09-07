using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
					Description = "ZWrite 显示数组与 Shader 值数组拆开",
					FileName = "ZBufferOpHelper.cs",
					Marker = "ZWriteModeLabels",
					Find = "m_zWriteMode.EnumTypePopup( ref owner, ZWriteModeStr, ZWriteModeValues );",
					Replace = "m_zWriteMode.EnumTypePopup( ref owner, ZWriteModeStr, ZWriteModeLabels );"
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
			var catalog = Catalog();
			for( int i = 0; i < catalog.Count; i++ )
				results.Add( Evaluate( catalog[ i ], false ) );
			return results;
		}

		public static List<ASEZHPatchResult> ApplyAll()
		{
			var results = new List<ASEZHPatchResult>();
			var catalog = Catalog();
			for( int i = 0; i < catalog.Count; i++ )
				results.Add( Evaluate( catalog[ i ], true ) );
			AssetDatabase.Refresh();
			return results;
		}

		static ASEZHPatchResult Evaluate( ASEZHPatch patch, bool apply )
		{
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
			if( !string.IsNullOrEmpty( patch.Marker ) && text.Contains( patch.Marker ) )
			{
				result.Status = "applied";
				result.Detail = patch.Description;
				return result;
			}
			if( string.IsNullOrEmpty( patch.Find ) || !text.Contains( patch.Find ) )
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
			string next = text.Replace( patch.Find, patch.Replace );
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

		static string ToAbsolute( string assetPath )
		{
			string root = Path.GetDirectoryName( Application.dataPath );
			return Path.GetFullPath( Path.Combine( root, assetPath ) );
		}
	}
}
