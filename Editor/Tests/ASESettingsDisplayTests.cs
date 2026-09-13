using NUnit.Framework;
using UnityEngine;

namespace AmplifyShaderEditor.Tests
{
	public class ASESettingsDisplayTests
	{
		bool m_previous;
		[SetUp] public void SetUp() { m_previous = ASELocale.UseChinese; ASELocale.UseChinese = true; ASELocale.Reload(); }
		[TearDown] public void TearDown() { ASELocale.UseChinese = m_previous; }

		[TestCase( "Common Properties", "通用属性" )]
		[TestCase( "Edit Template", "编辑模板" )]
		[TestCase( "Alpha Clipping", "透明度裁剪" )]
		[TestCase( "Cotton Wool", "棉毛" )]
		[TestCase( "  Receive SSR", "  接收屏幕空间反射" )]
		public void NativeSettingsCaption( string key, string expected )
		{
			Assert.That( ASESettingsDisplay.Label( key ), Is.EqualTo( expected ) );
			ASELocale.UseChinese = false;
			Assert.That( ASESettingsDisplay.Label( key ), Is.EqualTo( key ) );
		}

		[Test]
		public void ContentAndOptionsAreClonedWithoutChangingIdentifiersOrIndices()
		{
			var original = new GUIContent( "Shader Type", "Specify the shader type you want to be working on" );
			var shown = ASESettingsDisplay.Label( original );
			Assert.That( shown, Is.Not.SameAs( original ) );
			Assert.That( shown.tooltip, Is.EqualTo( "选择要编辑的着色器类型" ) );
			Assert.That( original.text, Is.EqualTo( "Shader Type" ) );
			var keys = new[] { "Cotton Wool", "Silk", "_MyShaderProperty", "Vulkan" };
			var labels = ASELocale.TranslateArray( keys );
			Assert.That( labels, Is.EqualTo( new[] { "棉毛", "丝绸", "_MyShaderProperty", "Vulkan" } ) );
			Assert.That( keys[ 0 ], Is.EqualTo( "Cotton Wool" ) );
			Assert.That( labels.Length, Is.EqualTo( keys.Length ) );
		}
	}
}
