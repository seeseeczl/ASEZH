using System.Collections.Generic;

namespace AmplifyShaderEditor
{
	internal static class ASESettingsPatchCatalog
	{
		internal static void AddTo( List<ASEZHPatch> patches )
		{
			int index = 0;
			foreach( string[] row in Rows ) patches.Add( new ASEZHPatch {
				Id = "settings-direct-" + index++, FileName = row[ 0 ], Description = "设置面板显示：" + row[ 0 ],
				Find = row[ 1 ], Replace = row[ 2 ], Marker = row[ 2 ], ReplaceAll = true } );
		}
		static readonly string[][] Rows =
		{
			new[] { "TemplateMasterNode.cs", "EditorGUILayout.HelpBox( WarningMessage, MessageType.Warning )", "EditorGUILayout.HelpBox( ASESettingsDisplay.Label( WarningMessage ), MessageType.Warning )" },
			new[] { "TemplateMultiPassMasterNode.cs", "EditorGUILayout.HelpBox( WarningMessage, MessageType.Warning )", "EditorGUILayout.HelpBox( ASESettingsDisplay.Label( WarningMessage ), MessageType.Warning )" },
			new[] { "TemplateColorMaskModule.cs", "EditorGUILayout.LabelField( \"Color Mask\"+ m_target,", "EditorGUILayout.LabelField( ASESettingsDisplay.Label( \"Color Mask\" ) + m_target," },
			new[] { "NodeUtils.cs", "GUILayout.Toggle( foldoutValue, sectionName,", "GUILayout.Toggle( foldoutValue, ASESettingsDisplay.Label( sectionName )," },
			new[] { "NodeUtils.cs", "GUI.Toggle( tog, foldoutValue, sectionName,", "GUI.Toggle( tog, foldoutValue, ASESettingsDisplay.Label( sectionName )," },
			new[] { "NodeParametersWindow.cs", "m_dummyContent.text = \"Output Node\";", "m_dummyContent.text = ASESettingsDisplay.Label( \"Output Node\" );" },
			new[] { "TemplateMasterNode.cs", "GUILayout.Button( OpenTemplateStr )", "GUILayout.Button( ASESettingsDisplay.Label( OpenTemplateStr ) )" },
			new[] { "TemplateMultiPassMasterNode.cs", "GUILayout.Button( OpenTemplateStr )", "GUILayout.Button( ASESettingsDisplay.Label( OpenTemplateStr ) )" },
			new[] { "TemplateMultiPassMasterNode.cs", "GUILayout.Button( ReloadTemplateStr )", "GUILayout.Button( ASESettingsDisplay.Label( ReloadTemplateStr ) )" },
			new[] { "TemplateMultiPassMasterNode.cs", "EditorGUILayout.HelpBox( NoSubShaderPropertyStr ,", "EditorGUILayout.HelpBox( ASESettingsDisplay.Label( NoSubShaderPropertyStr ) ," },
			new[] { "ColorMaskHelper.cs", "EditorGUILayout.LabelField( ColorMaskContent,", "EditorGUILayout.LabelField( ASESettingsDisplay.Label( ColorMaskContent )," },
			new[] { "StandardSurface.cs", "GUILayout.Toggle( ContainerGraph.ParentWindow.InnerWindowVariables.ExpandedBlendOptions, AlphaModeContent,", "GUILayout.Toggle( ContainerGraph.ParentWindow.InnerWindowVariables.ExpandedBlendOptions, ASESettingsDisplay.Label( AlphaModeContent )," },
			new[] { "TessellationOpHelper.cs", "GUILayout.Toggle( m_parentSurface.ContainerGraph.ParentWindow.InnerWindowVariables.ExpandedTesselation, \" Tessellation\",", "GUILayout.Toggle( m_parentSurface.ContainerGraph.ParentWindow.InnerWindowVariables.ExpandedTesselation, ASESettingsDisplay.Label( \" Tessellation\" )," },
			new[] { "MasterNode.cs", "EditorGUILayout.Foldout( m_shaderKeywordsFoldout, ShaderKeywordsStr )", "EditorGUILayout.Foldout( m_shaderKeywordsFoldout, ASESettingsDisplay.Label( ShaderKeywordsStr ) )" },
			new[] { "CustomTagsHelper.cs", "EditorGUILayout.TextField( TagNameStr,", "EditorGUILayout.TextField( ASESettingsDisplay.Label( TagNameStr )," },
			new[] { "CustomTagsHelper.cs", "EditorGUILayout.TextField( TagValueStr,", "EditorGUILayout.TextField( ASESettingsDisplay.Label( TagValueStr )," },
			new[] { "DependenciesHelper.cs", "EditorGUILayout.TextField( DependencyNameStr,", "EditorGUILayout.TextField( ASESettingsDisplay.Label( DependencyNameStr )," },
			new[] { "DependenciesHelper.cs", "EditorGUILayout.TextField( DependencyValueStr,", "EditorGUILayout.TextField( ASESettingsDisplay.Label( DependencyValueStr )," },
			new[] { "ZBufferOpHelper.cs", "EditorGUILayout.HelpBox( \"Depth Writing is only available for Opaque or Custom blend modes\",", "EditorGUILayout.HelpBox( ASESettingsDisplay.Label( \"Depth Writing is only available for Opaque or Custom blend modes\" )," },
			new[] { "StandardSurface.cs", "EditorGUILayout.HelpBox( \"Advanced options are only available for Custom blend modes\",", "EditorGUILayout.HelpBox( ASESettingsDisplay.Label( \"Advanced options are only available for Custom blend modes\" )," },
			new[] { "AdditionalPragmasHelper.cs", "EditorGUILayout.HelpBox( \"Please add your pragmas without the #pragma keywords\",", "EditorGUILayout.HelpBox( ASESettingsDisplay.Label( \"Please add your pragmas without the #pragma keywords\" )," },
			new[] { "AdditionalDefinesHelper.cs", "EditorGUILayout.HelpBox( \"Please add your defines without the #define keywords\",", "EditorGUILayout.HelpBox( ASESettingsDisplay.Label( \"Please add your defines without the #define keywords\" )," },
			new[] { "AdditionalIncludesHelper.cs", "EditorGUILayout.HelpBox( \"Please add your includes without the #include \\\"\\\" keywords\",", "EditorGUILayout.HelpBox( ASESettingsDisplay.Label( \"Please add your includes without the #include \\\"\\\" keywords\" )," },
			new[] { "TemplateTagsModule.cs", "EditorGUILayout.HelpBox( \"Your list is Empty!\\nUse the plus button to add one.\",", "EditorGUILayout.HelpBox( ASESettingsDisplay.Label( \"Your list is Empty!\\nUse the plus button to add one.\" )," },
			new[] { "TemplateAdditionalDirectivesHelper.cs", "EditorGUILayout.HelpBox( \"Your list is Empty!\\nUse the plus button to add one.\",", "EditorGUILayout.HelpBox( ASESettingsDisplay.Label( \"Your list is Empty!\\nUse the plus button to add one.\" )," },
			new[] { "CustomTagsHelper.cs", "EditorGUILayout.HelpBox( \"Your list is Empty!\\nUse the plus button to add one.\",", "EditorGUILayout.HelpBox( ASESettingsDisplay.Label( \"Your list is Empty!\\nUse the plus button to add one.\" )," },
			new[] { "AdditionalSurfaceOptionsHelper.cs", "EditorGUILayout.HelpBox( \"Your list is Empty!\\nUse the plus button to add one.\",", "EditorGUILayout.HelpBox( ASESettingsDisplay.Label( \"Your list is Empty!\\nUse the plus button to add one.\" )," },
			new[] { "DependenciesHelper.cs", "EditorGUILayout.HelpBox( \"Your list is Empty!\\nUse the plus button to add one.\",", "EditorGUILayout.HelpBox( ASESettingsDisplay.Label( \"Your list is Empty!\\nUse the plus button to add one.\" )," },
			new[] { "UsePassHelper.cs", "EditorGUILayout.HelpBox( \"Your list is Empty!\\nUse the plus button to add one.\",", "EditorGUILayout.HelpBox( ASESettingsDisplay.Label( \"Your list is Empty!\\nUse the plus button to add one.\" )," },
			new[] { "TemplateAdditionalDirectivesHelper.cs", "EditorGUI.LabelField( condPos, \"to\" )", "EditorGUI.LabelField( condPos, ASESettingsDisplay.Label( \"to\" ) )" },
			new[] { "TemplateAdditionalDirectivesHelper.cs", "EditorGUI.LabelField( condPos, new GUIContent( \"Passes\", \"Template pass names separated by semicolon (;). Empty means it will be included in all passes.\" ) )", "EditorGUI.LabelField( condPos, ASESettingsDisplay.Label( new GUIContent( \"Passes\", \"Template pass names separated by semicolon (;). Empty means it will be included in all passes.\" ) ) )" },
			new[] { "TemplateAdditionalDirectivesHelper.cs", "EditorGUI.LabelField( condPos, new GUIContent( \"SRPVersion\", \"Valid SRP version numbers must have 6 digits and be equal or higher than 100000, the lowest supported version.\" ) )", "EditorGUI.LabelField( condPos, ASESettingsDisplay.Label( new GUIContent( \"SRPVersion\", \"Valid SRP version numbers must have 6 digits and be equal or higher than 100000, the lowest supported version.\" ) ) )" },
		};
	}
}
