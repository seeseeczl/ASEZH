using System.Collections.Generic;

namespace AmplifyShaderEditor
{
	internal static class ASENativePatchCatalog
	{
		internal static void AddTo( List<ASEZHPatch> patches )
		{
			foreach( ASEZHPatch patch in patches )
			{
				if( patch.Id != "palette-node-item" && patch.Id != "palette-node-item-content" ) continue;
				patch.LegacyReplace = patch.Replace;
				patch.Replace = patch.Replace.Replace( "ASELocale.TNodeListLabel( current.Value.Contents[ i ].Name,",
					"ASENativeDisplay.ListLabel( current.Value.Contents[ i ].NodeType, current.Value.Contents[ i ].Category, current.Value.Contents[ i ].Name," );
				patch.Marker = "ASENativeDisplay.ListLabel(";
			}
			foreach( string direction in new[] { "input", "output" } )
			{
				string ports = "m_" + direction + "Ports[ i ]";
				string display = "ASENativeDisplay.Port( this, " + ( direction == "input" ? "true" : "false" )
					+ ", " + ports + ".PortId, " + ports + ".Name, " + ports + ".IsEditable )";
				string style = direction == "input" ? "InputPortLabel" : "OutputPortLabel";
				Add( patches, "native-" + direction + "-label", "ParentNode.cs",
					"GUI.Label( " + ports + ".LabelPosition, " + ports + ".Name, UIUtils." + style + " );",
					"GUI.Label( " + ports + ".LabelPosition, " + display + ", UIUtils." + style + " );" );
				Add( patches, "native-" + direction + "-measure", "ParentNode.cs",
					"m_sizeContentAux.text = " + ports + ".Name;", "m_sizeContentAux.text = " + display + ";" );
			}
			Add( patches, "native-title-measure", "ParentNode.cs", "UIUtils.UnZoomedNodeTitleStyle.CalcSize( m_content )",
				"UIUtils.UnZoomedNodeTitleStyle.CalcSize( ASENativeDisplay.TitleContent( this, m_content ) )" );
			string layoutGuard = "\n\t\t\tif( ASENativeDisplay.NeedsLayout( this ) )\n\t\t\t{\n\t\t\t\tm_sizeIsDirty = true;\n\t\t\t\tforeach( var port in m_inputPorts ) port.DirtyLabelSize = true;\n\t\t\t\tforeach( var port in m_outputPorts ) port.DirtyLabelSize = true;\n\t\t\t}";
			string layoutFind = "public virtual void OnNodeLayout( DrawInfo drawInfo )\n\t\t{";
			// ASE 1.9.9 起 OnNodeLayout 多出 NodeUpdateCache 参数：同一语义挂两套签名锚点，命中哪套就保留哪套签名行。
			string layoutAltFind = "public virtual void OnNodeLayout( DrawInfo drawInfo, NodeUpdateCache cache = null )\n\t\t{";
			ASEZHPatch layoutPatch = Add( patches, "native-language-layout", "ParentNode.cs", layoutFind, layoutFind + layoutGuard );
			layoutPatch.AltFind = layoutAltFind;
			layoutPatch.AltReplace = layoutAltFind + layoutGuard;
			foreach( string file in new[] { "PropertyNode.cs", "FunctionInput.cs", "FunctionOutput.cs" } )
				Add( patches, "native-title-" + file, file,
					"GUI.Label( m_titleClickArea, m_content, UIUtils.GetCustomStyle( CustomStyle.NodeTitle ) );",
					"GUI.Label( m_titleClickArea, ASENativeDisplay.TitleContent( this, m_content ), UIUtils.GetCustomStyle( CustomStyle.NodeTitle ) );" );
			Add( patches, "native-title-CustomExpressionNode.cs", "CustomExpressionNode.cs",
				"GUI.Label( m_titleClickArea , m_content , UIUtils.GetCustomStyle( CustomStyle.NodeTitle ) );",
				"GUI.Label( m_titleClickArea , ASENativeDisplay.TitleContent( this, m_content ) , UIUtils.GetCustomStyle( CustomStyle.NodeTitle ) );" );
			Add( patches, "native-panel-title", "NodeParametersWindow.cs", "m_dummyContent.text = selectedNode.Attributes.Name;",
				"m_dummyContent.text = ASENativeDisplay.Title( selectedNode, selectedNode.Attributes.Name );" );
			foreach( string file in new[] { "ScreenColorNode.cs", "StaticSwitch.cs" } )
			{
				string label = file == "StaticSwitch.cs" ? "StaticSwitchStr" : "\"Grab Screen Color\"";
				Add( patches, "native-title-" + file, file,
					"GUI.Label( titlePos, " + label + ", UIUtils.GetCustomStyle( CustomStyle.NodeTitle ) );",
					"GUI.Label( titlePos, ASENativeDisplay.Title( this, " + label + " ), UIUtils.GetCustomStyle( CustomStyle.NodeTitle ) );" );
			}
			Add( patches, "native-wire-label", "WireNode.cs",
				"GUI.Label( m_outputPorts[ i ].LabelPosition, m_outputPorts[ i ].Name, UIUtils.OutputPortLabel );",
				"GUI.Label( m_outputPorts[ i ].LabelPosition, ASENativeDisplay.Port( this, false, m_outputPorts[ i ].PortId, m_outputPorts[ i ].Name, m_outputPorts[ i ].IsEditable ), UIUtils.OutputPortLabel );" );
		}

		static ASEZHPatch Add( List<ASEZHPatch> patches, string id, string file, string find, string replace )
		{
			var patch = new ASEZHPatch { Id = id, FileName = file, Description = "原生节点显示：" + id,
				Find = find, Replace = replace, Marker = replace, ReplaceAll = id != "native-language-layout" };
			patches.Add( patch );
			return patch;
		}
	}
}
