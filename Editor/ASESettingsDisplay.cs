using UnityEngine;

namespace AmplifyShaderEditor
{
	/// <summary>Only accepts drawing captions, never editable values or Shader identifiers.</summary>
	public static class ASESettingsDisplay
	{
		public static string Label( string original ) { return ASELocale.T( original ); }
		public static GUIContent Label( GUIContent original )
		{
			return original == null ? null : new GUIContent( Label( original.text ), original.image, Label( original.tooltip ) );
		}
	}
}
