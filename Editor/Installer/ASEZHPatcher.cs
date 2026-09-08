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
		static AseInstallation s_activeInstallation;
		internal static ASEZHPatchSessionResult LastSession { get; private set; }
		public static string LastSessionStatus { get { return LastSession == null ? "none" : LastSession.State.ToString(); } }
		public static string LastSessionDetail { get { return LastSession == null ? string.Empty : LastSession.Detail; } }
		public static string LastSessionTarget { get { return LastSession == null ? string.Empty : LastSession.TargetRoot; } }
		public static string LastSessionBackup { get { return LastSession == null ? string.Empty : LastSession.BackupPath; } }
		public static bool LastSessionSucceeded { get { return LastSession != null && LastSession.IsSuccess; } }

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
			AseInstallation installation;
			string detail;
			return AseTargetResolver.TryResolve( out installation, out detail ) ? installation.AssetRoot : null;
		}

		public static string FindFile( string fileName )
		{
			if( s_activeInstallation != null )
				return s_activeInstallation.AssetPathFor( fileName );
			AseInstallation installation;
			string detail;
			return AseTargetResolver.TryResolve( out installation, out detail )
				? installation.AssetPathFor( fileName )
				: null;
		}

		public static List<ASEZHPatchResult> Scan()
		{
			AseInstallation installation;
			string detail;
			if( !AseTargetResolver.TryResolve( out installation, out detail ) )
			{
				LastSession = new ASEZHPatchSessionResult
				{
					State = ASEZHPatchSessionState.PreflightRejected,
					Detail = detail,
					Results = new List<ASEZHPatchResult>
					{
						new ASEZHPatchResult { Id = "target-preflight", File = "Amplify Shader Editor", Status = "mismatch", Detail = detail }
					}
				};
				return LastSession.Results;
			}
			List<ASEZHPatchResult> results = RunScanUnsafe( installation );
			LastSession = new ASEZHPatchSessionResult
			{
				State = ASEZHPatchSessionState.NoOp,
				TargetRoot = installation.AssetRoot,
				Detail = detail,
				Results = results
			};
			return results;
		}

		public static List<ASEZHPatchResult> ApplyAll()
		{
			LastSession = ASEZHPatchTransaction.Execute( true );
			if( LastSession.State == ASEZHPatchSessionState.Succeeded )
				AssetDatabase.Refresh();
			return LastSession.Results;
		}

		public static List<ASEZHPatchResult> RemoveAll()
		{
			LastSession = ASEZHPatchTransaction.Execute( false );
			if( LastSession.State == ASEZHPatchSessionState.Succeeded )
				AssetDatabase.Refresh();
			return LastSession.Results;
		}

		internal static List<ASEZHPatchResult> RunUnsafe( AseInstallation installation, bool apply )
		{
			AseInstallation previous = s_activeInstallation;
			s_activeInstallation = installation;
			try
			{
				var results = new List<ASEZHPatchResult>();
				if( apply )
				{
					results.Add( ASEZHPatchTransforms.EnsureAseAssemblyReference( true ) );
					List<ASEZHPatch> catalog = Catalog();
					for( int i = 0; i < catalog.Count; i++ )
						results.Add( ASEZHPatchTransforms.Evaluate( catalog[ i ], true ) );
				}
				else
				{
					List<ASEZHPatch> catalog = Catalog();
					for( int i = catalog.Count - 1; i >= 0; i-- )
						results.Add( ASEZHPatchTransforms.Reverse( catalog[ i ] ) );
					results.Add( ASEZHPatchTransforms.ReverseAseAssemblyReference() );
				}
				return results;
			}
			finally
			{
				s_activeInstallation = previous;
			}
		}

		static List<ASEZHPatchResult> RunScanUnsafe( AseInstallation installation )
		{
			AseInstallation previous = s_activeInstallation;
			s_activeInstallation = installation;
			try
			{
				var results = new List<ASEZHPatchResult>();
				results.Add( ASEZHPatchTransforms.EnsureAseAssemblyReference( false ) );
				List<ASEZHPatch> catalog = Catalog();
				for( int i = 0; i < catalog.Count; i++ )
					results.Add( ASEZHPatchTransforms.Evaluate( catalog[ i ], false ) );
				return results;
			}
			finally
			{
				s_activeInstallation = previous;
			}
		}

		const string LocaleAssemblyName = "ASEZH.Editor";

		internal static string FindAseAsmdef()
		{
			if( s_activeInstallation != null )
				return s_activeInstallation.AssemblyDefinitionAssetPath;
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
			return ASEZHPatchTransforms.EnsureAseAssemblyReference( apply );
		}

		internal static string ToAbsolute( string assetPath )
		{
			if( s_activeInstallation != null )
			{
				if( assetPath == s_activeInstallation.AssemblyDefinitionAssetPath )
					return s_activeInstallation.AssemblyDefinitionAbsolutePath;
				string mapped = s_activeInstallation.AbsolutePathFor( Path.GetFileName( assetPath ) );
				if( !string.IsNullOrEmpty( mapped ) )
					return mapped;
			}
			return AseTargetResolver.ToAbsolute( assetPath );
		}
	}
}
