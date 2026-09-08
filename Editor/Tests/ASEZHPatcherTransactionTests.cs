using System.Collections.Generic;
using System.IO;
using System.Text;
using NUnit.Framework;

namespace AmplifyShaderEditor.Tests
{
	public class ASEZHPatcherTransactionTests
	{
		string m_root;

		[SetUp]
		public void SetUp()
		{
			m_root = Path.Combine( Path.GetTempPath(), "asezh-tests-" + System.Guid.NewGuid().ToString( "N" ) );
			Directory.CreateDirectory( m_root );
		}

		[TearDown]
		public void TearDown()
		{
			if( Directory.Exists( m_root ) )
				Directory.Delete( m_root, true );
		}

		[TestCase( "\n", "    ", false )]
		[TestCase( "\r\n", "\t", true )]
		public void RemovePaletteBuildList_RestoresBraceAndBraceLessForms( string newline, string indent, bool braces )
		{
			string body = indent + "if( ASELocale.MatchesSearch( m_searchFilter, allItems[ i ].Name, allItems[ i ].Category, allItems[ i ].Tags ) )";
			if( braces )
				body += newline + indent + "{" + newline + indent + "    AddItem();" + newline + indent + "}";
			else
				body += newline + indent + "    AddItem();";

			string output;
			Assert.That( ASEZHSpecialPatchTransforms.TryRemovePaletteBuildListText( body, out output ), Is.True );
			Assert.That( output, Does.Not.Contain( "ASELocale.MatchesSearch" ) );
			Assert.That( output, Does.Contain( "allItems[ i ].Name.IndexOf" ) );
			Assert.That( output, Does.Contain( "allItems[ i ].Category.IndexOf" ) );
			Assert.That( output, Does.Contain( newline ) );
		}

		[Test]
		public void Commit_WhenSecondWriteFails_RestoresEveryTouchedPreimage()
		{
			string first = CreateFile( "first.cs", "first-before" );
			string second = CreateFile( "second.cs", "second-before" );
			string backup = Path.Combine( m_root, "backup" );
			Directory.CreateDirectory( backup );
			var plans = new List<PatchFilePlan>
			{
				ASEZHPatchTransaction.CreatePlanForTests( first, Path.Combine( backup, "first.cs" ), Bytes( "first-after" ) ),
				ASEZHPatchTransaction.CreatePlanForTests( second, Path.Combine( backup, "second.cs" ), Bytes( "second-after" ) )
			};

			string detail;
			ASEZHPatchSessionState state = ASEZHPatchTransaction.CommitForTests( plans, 1, out detail );

			Assert.That( state, Is.EqualTo( ASEZHPatchSessionState.FailedRestored ) );
			Assert.That( File.ReadAllText( first ), Is.EqualTo( "first-before" ) );
			Assert.That( File.ReadAllText( second ), Is.EqualTo( "second-before" ) );
			Assert.That( detail, Does.Contain( "已恢复" ) );
		}

		[Test]
		public void Commit_WhenAllWritesSucceed_WritesEveryPlannedOutput()
		{
			string first = CreateFile( "first.cs", "first-before" );
			string second = CreateFile( "second.cs", "second-before" );
			string backup = Path.Combine( m_root, "backup" );
			Directory.CreateDirectory( backup );
			var plans = new List<PatchFilePlan>
			{
				ASEZHPatchTransaction.CreatePlanForTests( first, Path.Combine( backup, "first.cs" ), Bytes( "first-after" ) ),
				ASEZHPatchTransaction.CreatePlanForTests( second, Path.Combine( backup, "second.cs" ), Bytes( "second-after" ) )
			};

			string detail;
			ASEZHPatchSessionState state = ASEZHPatchTransaction.CommitForTests( plans, -1, out detail );

			Assert.That( state, Is.EqualTo( ASEZHPatchSessionState.Succeeded ) );
			Assert.That( File.ReadAllText( first ), Is.EqualTo( "first-after" ) );
			Assert.That( File.ReadAllText( second ), Is.EqualTo( "second-after" ) );
			Assert.That( Directory.GetFiles( m_root, "*.tmp", SearchOption.AllDirectories ), Is.Empty );
		}

		[Test]
		public void TargetResolver_KeepsTwoCompleteInstallationsAsTwoRoots()
		{
			var required = new List<string> { "UndoParentNode.cs", "PaletteParent.cs" };
			var candidates = new Dictionary<string, List<string>>
			{
				{ "UndoParentNode.cs", new List<string> { "Assets/ASE-A/Editor/UndoParentNode.cs", "Assets/ASE-B/Editor/UndoParentNode.cs" } },
				{ "PaletteParent.cs", new List<string> { "Assets/ASE-A/Editor/PaletteParent.cs", "Assets/ASE-B/Editor/PaletteParent.cs" } }
			};
			string firstRoot;
			string secondRoot;
			Dictionary<string, string> firstFiles;
			Dictionary<string, string> secondFiles;

			Assert.That( AseTargetResolver.TryFindSmallestCompleteRoot(
				candidates[ "UndoParentNode.cs" ][ 0 ], required, candidates, out firstRoot, out firstFiles ), Is.True );
			Assert.That( AseTargetResolver.TryFindSmallestCompleteRoot(
				candidates[ "UndoParentNode.cs" ][ 1 ], required, candidates, out secondRoot, out secondFiles ), Is.True );
			Assert.That( firstRoot, Is.EqualTo( "Assets/ASE-A/Editor" ) );
			Assert.That( secondRoot, Is.EqualTo( "Assets/ASE-B/Editor" ) );
			Assert.That( firstRoot, Is.Not.EqualTo( secondRoot ) );
		}

		[Test]
		public void TargetResolver_RejectsCrossRootRequiredFiles()
		{
			var required = new List<string> { "UndoParentNode.cs", "PaletteParent.cs" };
			var candidates = new Dictionary<string, List<string>>
			{
				{ "UndoParentNode.cs", new List<string> { "Assets/ASE-A/Editor/UndoParentNode.cs" } },
				{ "PaletteParent.cs", new List<string> { "Assets/ASE-B/Editor/PaletteParent.cs" } }
			};
			string root;
			Dictionary<string, string> files;

			Assert.That( AseTargetResolver.TryFindSmallestCompleteRoot(
				candidates[ "UndoParentNode.cs" ][ 0 ], required, candidates, out root, out files ), Is.False );
		}

		string CreateFile( string name, string content )
		{
			string path = Path.Combine( m_root, name );
			File.WriteAllText( path, content, new UTF8Encoding( false ) );
			return path;
		}

		static byte[] Bytes( string value )
		{
			return new UTF8Encoding( false ).GetBytes( value );
		}
	}
}
