using System.Collections.Generic;

namespace AmplifyShaderEditor
{
	/// <summary>
	/// 节点检视面板、画布与 Inspector 上仍然直接显示英文的固定文案。
	/// 只包显示参数，不碰 TextField 值、Shader 标识符与平台名。
	/// </summary>
	internal static class ASEUiCaptionPatchCatalog
	{
		internal static void AddTo( List<ASEZHPatch> patches )
		{
			int index = 0;
			foreach( string[] row in Rows )
				patches.Add( new ASEZHPatch {
					Id = "ui-caption-" + index++, FileName = row[ 0 ], Description = "界面文案：" + row[ 0 ],
					Find = row[ 1 ], Replace = row[ 2 ], Marker = row[ 2 ], ReplaceAll = true } );
		}

		static readonly string[][] Rows =
		{
			new[] { "PropertyNode.cs", "EditorGUILayout.HelpBox( \"Your list is Empty!\\nUse the plus button to add more.\", MessageType.Info )", "EditorGUILayout.HelpBox( ASELocale.T( \"Your list is Empty!\\nUse the plus button to add more.\" ), MessageType.Info )" },
			new[] { "NodeParametersWindow.cs", "new GUIContent( \"    Description\" )", "new GUIContent( ASELocale.T( \"    Description\" ) )" },
			new[] { "NodeParametersWindow.cs", "new GUIContent( \"Custom URL\" )", "new GUIContent( ASELocale.T( \"Custom URL\" ) )" },
			new[] { "TriplanarNode.cs", "\"None (Texture2D)\", UIUtils.MiniObjectFieldThumbOverlay )", "ASELocale.T( \"None (Texture2D)\" ), UIUtils.MiniObjectFieldThumbOverlay )" },
			new[] { "TriplanarNode.cs", "GUI.Label( pickerButtonClone, \"Select\", UIUtils.MiniSamplerButton );", "GUI.Label( pickerButtonClone, ASELocale.T( \"Select\" ), UIUtils.MiniSamplerButton );" },
			new[] { "CustomShaderInspector.cs", "new GUIContent( \"Compile and show code | ▾\" )", "new GUIContent( ASELocale.T( \"Compile and show code | ▾\" ) )" },
			new[] { "CustomShaderInspector.cs", "GUILayout.Button( \"Open in Shader Editor\" )", "GUILayout.Button( ASELocale.T( \"Open in Shader Editor\" ) )" },
			new[] { "CustomMaterialInspector.cs", "GUILayout.Button( \"Open in Shader Editor\" )", "GUILayout.Button( ASELocale.T( \"Open in Shader Editor\" ) )" },
			new[] { "CustomShaderInspector.cs", "GUILayout.Button( \"Open in Text Editor\" )", "GUILayout.Button( ASELocale.T( \"Open in Text Editor\" ) )" },
			new[] { "FunctionOutput.cs", "GUILayout.Button( \"Set as Preview\" )", "GUILayout.Button( ASELocale.T( \"Set as Preview\" ) )" },
			new[] { "RegisterLocalVarNode.cs", "EditorGUILayout.HelpBox( \"This node is not being referenced by any Get Local Var.\", MessageType.Info )", "EditorGUILayout.HelpBox( ASELocale.T( \"This node is not being referenced by any Get Local Var.\" ), MessageType.Info )" },
			new[] { "PortLegendInfo.cs", "new GUIContent( \"Helper\", \"Shows helper info for ASE users\" )", "new GUIContent( ASELocale.T( \"Helper\" ), ASELocale.T( \"Shows helper info for ASE users\" ) )" },
			new[] { "PortLegendInfo.cs", "GUILayout.Button( \"Wiki Page\" )", "GUILayout.Button( ASELocale.T( \"Wiki Page\" ) )" },
		};
	}
}
