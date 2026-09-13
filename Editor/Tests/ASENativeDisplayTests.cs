using NUnit.Framework;
using UnityEngine;

namespace AmplifyShaderEditor.Tests
{
	public class ASENativeDisplayTests
	{
		[TestCase( "Flyme" )]
		[TestCase( "FLYME/效果" )]
		[TestCase( "Custom/FlyMe/Math" )]
		public void FlymeVetoWinsEvenForNativeType( string category )
		{
			Assert.IsFalse( ASENativeDisplay.IsNative( "AmplifyShaderEditor.SimpleMultiplyOpNode", category ) );
		}

		[Test]
		public void OnlyExactNativeTypesAreEligible()
		{
			Assert.IsTrue( ASENativeDisplay.IsNative( "AmplifyShaderEditor.SimpleMultiplyOpNode", "Math Operators" ) );
			Assert.IsTrue( ASENativeDisplay.IsNative( "AmplifyShaderEditor.TemplateMultiPassMasterNode", "Master" ) );
			Assert.IsFalse( ASENativeDisplay.IsNative( "Custom.SimpleMultiplyOpNode", "Math Operators" ) );
			Assert.IsFalse( ASENativeDisplay.IsNative( "AmplifyShaderEditor.FunctionNode", "Functions" ) );
			Assert.IsFalse( ASENativeDisplay.IsNative( "AmplifyShaderEditor.SimpleMultiplyOpNode", null ) );
		}

		[Test]
		public void UnknownObjectsAndSourceContentStayUntouched()
		{
			bool previous = ASELocale.UseChinese;
			try
			{
				ASELocale.UseChinese = true;
				object custom = new object();
				GUIContent source = new GUIContent( "Color", "original tooltip" );
				GUIContent shown = ASENativeDisplay.TitleContent( custom, source );
				Assert.AreEqual( "Color", shown.text );
				Assert.AreEqual( "Color", source.text );
				Assert.AreEqual( source.tooltip, shown.tooltip );
				Assert.AreEqual( "Normal", ASENativeDisplay.Port( custom, true, 0, "Normal", false ) );
				Assert.AreEqual( "Normal", ASENativeDisplay.Port( null, true, 0, "Normal", false ) );
				Assert.AreEqual( "法线", ASELocale.T( "Normal", "port_label" ) );
				Assert.AreEqual( "R", ASELocale.T( "R", "port_label" ) );
				Assert.AreEqual( "UV", ASELocale.T( "UV", "port_label" ) );
			}
			finally { ASELocale.UseChinese = previous; }
		}
	}
}
