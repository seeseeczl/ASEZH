using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace AmplifyShaderEditor
{
	internal sealed class PatchReceiptEntry
	{
		internal string TargetPath;
		internal string PreimagePath;
		internal string AppliedHash;
	}

	internal static class ASEZHPatchReceiptStore
	{
		static string ReceiptRoot( string projectRoot, AseInstallation target )
		{
			string key = Sha256( Encoding.UTF8.GetBytes( target.AssetRoot ?? target.AbsoluteRoot ) ).Substring( 0, 24 );
			return Path.Combine( projectRoot, "Library", "ASEZH", "Installed", key );
		}

		internal static void Save( string projectRoot, AseInstallation target, List<PatchFilePlan> plans )
		{
			string root = ReceiptRoot( projectRoot, target );
			if( Directory.Exists( root ) )
				Directory.Delete( root, true );
			Directory.CreateDirectory( root );
			var lines = new List<string> { "schema=1", "target=" + Convert.ToBase64String( Encoding.UTF8.GetBytes( target.AssetRoot ) ) };
			for( int i = 0; i < plans.Count; i++ )
			{
				string preimageName = i.ToString( "D2" ) + "-" + Path.GetFileName( plans[ i ].TargetPath );
				File.WriteAllBytes( Path.Combine( root, preimageName ), plans[ i ].Preimage );
				lines.Add( Convert.ToBase64String( Encoding.UTF8.GetBytes( plans[ i ].TargetPath ) )
					+ "\t" + preimageName + "\t" + plans[ i ].PreimageHash + "\t" + plans[ i ].OutputHash );
			}
			File.WriteAllLines( Path.Combine( root, "receipt.tsv" ), lines.ToArray(), new UTF8Encoding( false ) );
		}

		internal static List<PatchReceiptEntry> Load( string projectRoot, AseInstallation target, out string error )
		{
			error = null;
			string root = ReceiptRoot( projectRoot, target );
			string manifest = Path.Combine( root, "receipt.tsv" );
			if( !File.Exists( manifest ) )
				return null;
			string[] lines = File.ReadAllLines( manifest, Encoding.UTF8 );
			var receipt = new List<PatchReceiptEntry>();
			for( int i = 2; i < lines.Length; i++ )
			{
				string[] fields = lines[ i ].Split( '\t' );
				if( fields.Length != 4 )
				{
					error = "回执格式无效。";
					return null;
				}
				string targetPath = Encoding.UTF8.GetString( Convert.FromBase64String( fields[ 0 ] ) );
				string preimagePath = Path.Combine( root, fields[ 1 ] );
				if( !File.Exists( targetPath ) || !File.Exists( preimagePath ) )
				{
					error = "回执文件缺失：" + targetPath;
					return null;
				}
				if( Sha256( File.ReadAllBytes( targetPath ) ) != fields[ 3 ] )
				{
					error = "接入后文件已被修改，无法安全恢复批准的 preimage：" + targetPath;
					return null;
				}
				if( Sha256( File.ReadAllBytes( preimagePath ) ) != fields[ 2 ] )
				{
					error = "回执 preimage 哈希不符：" + preimagePath;
					return null;
				}
				receipt.Add( new PatchReceiptEntry
				{
					TargetPath = targetPath,
					PreimagePath = preimagePath,
					AppliedHash = fields[ 3 ]
				} );
			}
			return receipt;
		}

		internal static void RestoreIntoPlan( List<PatchReceiptEntry> receipt, AseInstallation target, AseInstallation planned )
		{
			for( int i = 0; i < receipt.Count; i++ )
			{
				string plannedPath;
				if( receipt[ i ].TargetPath == target.AssemblyDefinitionAbsolutePath )
					plannedPath = planned.AssemblyDefinitionAbsolutePath;
				else
					plannedPath = planned.AbsolutePathFor( Path.GetFileName( receipt[ i ].TargetPath ) );
				if( string.IsNullOrEmpty( plannedPath ) )
					throw new IOException( "回执目标不属于当前 ASE 安装：" + receipt[ i ].TargetPath );
				File.WriteAllBytes( plannedPath, File.ReadAllBytes( receipt[ i ].PreimagePath ) );
			}
		}

		internal static void Delete( string projectRoot, AseInstallation target )
		{
			string root = ReceiptRoot( projectRoot, target );
			try
			{
				if( Directory.Exists( root ) )
					Directory.Delete( root, true );
			}
			catch
			{
				// A stale receipt never permits writes: its applied hashes must match first.
			}
		}

		static string Sha256( byte[] bytes )
		{
			using( SHA256 sha = SHA256.Create() )
			{
				byte[] hash = sha.ComputeHash( bytes );
				var builder = new StringBuilder( hash.Length * 2 );
				for( int i = 0; i < hash.Length; i++ )
					builder.Append( hash[ i ].ToString( "x2" ) );
				return builder.ToString();
			}
		}
	}
}
