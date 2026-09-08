using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
namespace AmplifyShaderEditor
{
	internal enum ASEZHPatchSessionState
	{
		Succeeded,
		NoOp,
		PreflightRejected,
		FailedRestored,
		FailedRecoveryIncomplete
	}
	internal sealed class ASEZHPatchSessionResult
	{
		internal string SessionId;
		internal string TargetRoot;
		internal string BackupPath;
		internal string Detail;
		internal ASEZHPatchSessionState State;
		internal List<ASEZHPatchResult> Results = new List<ASEZHPatchResult>();

		internal bool IsSuccess
		{
			get { return State == ASEZHPatchSessionState.Succeeded || State == ASEZHPatchSessionState.NoOp; }
		}
	}
	internal sealed class PatchFilePlan
	{
		internal string TargetPath;
		internal string BackupPath;
		internal byte[] Preimage;
		internal byte[] Output;
		internal string PreimageHash;
		internal string OutputHash;
	}
	internal static class ASEZHPatchTransaction
	{
		static int s_failAfterWrites = -1;
		internal static ASEZHPatchSessionResult Execute( bool apply )
		{
			string sessionId = DateTime.UtcNow.ToString( "yyyyMMddTHHmmssfff" ) + "-" + Guid.NewGuid().ToString( "N" ).Substring( 0, 8 );
			AseInstallation target;
			string targetDetail;
			if( !AseTargetResolver.TryResolve( out target, out targetDetail ) )
				return Rejected( sessionId, targetDetail );

			string projectRoot = Path.GetDirectoryName( UnityEngine.Application.dataPath );
			string sessionRoot = Path.Combine( projectRoot, "Library", "ASEZH", "PatchSessions", sessionId );
			string backupRoot = Path.Combine( sessionRoot, "preimage" );
			string planRoot = Path.Combine( sessionRoot, "plan" );
			var session = new ASEZHPatchSessionResult
			{
				SessionId = sessionId,
				TargetRoot = target.AssetRoot,
				BackupPath = backupRoot,
				Detail = targetDetail
			};
			try
			{
				Directory.CreateDirectory( backupRoot );
				Directory.CreateDirectory( planRoot );
				AseInstallation plannedInstallation = CreatePlanningCopy( target, planRoot );
				session.Results = ASEZHPatcher.RunUnsafe( plannedInstallation, apply );
				if( !PreflightPlan( session, projectRoot, target, plannedInstallation, apply ) )
					return session;

				List<PatchFilePlan> plans = BuildCommitPlan( target, plannedInstallation, backupRoot );
				if( plans.Count == 0 )
				{
					session.State = ASEZHPatchSessionState.NoOp;
					session.Detail = "预检通过，所有受管文件已处于目标状态，未写入。";
					TryWriteManifest( sessionRoot, session, plans );
					return session;
				}

				CommitSession( session, projectRoot, target, plans, apply );
				TryWriteManifest( sessionRoot, session, plans );
				return session;
			}
			catch( Exception exception )
			{
				session.State = ASEZHPatchSessionState.PreflightRejected;
				session.Detail = "补丁计划失败，目标未写入：" + exception.Message;
				return session;
			}
			finally
			{
				TryDeleteDirectory( planRoot );
			}
		}
		static bool PreflightPlan(
			ASEZHPatchSessionResult session,
			string projectRoot,
			AseInstallation target,
			AseInstallation planned,
			bool apply )
		{
			string invalid;
			if( HasInvalidResults( session.Results, out invalid ) )
			{
				session.State = ASEZHPatchSessionState.PreflightRejected;
				session.Detail = "预检未通过，目标未写入：" + invalid;
				return false;
			}
			if( apply )
				return true;
			string receiptError;
			List<PatchReceiptEntry> receipt = ASEZHPatchReceiptStore.Load( projectRoot, target, out receiptError );
			if( !string.IsNullOrEmpty( receiptError ) )
			{
				session.State = ASEZHPatchSessionState.PreflightRejected;
				session.Detail = "安装回执校验失败，目标未写入：" + receiptError;
				return false;
			}
			if( receipt != null )
				ASEZHPatchReceiptStore.RestoreIntoPlan( receipt, target, planned );
			return ValidateRemovedPlan( session, planned );
		}
		static bool ValidateRemovedPlan( ASEZHPatchSessionResult session, AseInstallation planned )
		{
			string residual = FindResidualLocaleReference( planned );
			if( string.IsNullOrEmpty( residual ) )
				return true;
			session.State = ASEZHPatchSessionState.PreflightRejected;
			session.Detail = "撤回后仍有 ASELocale 钩子，已保留程序集引用且目标未写入：" + residual;
			session.Results.Add( new ASEZHPatchResult
			{
				Id = "remove-residual-check",
				File = residual,
				Status = "mismatch",
				Detail = session.Detail
			} );
			return false;
		}
		static void CommitSession(
			ASEZHPatchSessionResult session,
			string projectRoot,
			AseInstallation target,
			List<PatchFilePlan> plans,
			bool apply )
		{
			ASEZHPatchSessionState commitState;
			string commitDetail;
			if( apply )
				ASEZHPatchReceiptStore.Save( projectRoot, target, plans );
			Commit( plans, s_failAfterWrites, out commitState, out commitDetail );
			session.State = commitState;
			session.Detail = commitDetail;
			if( commitState == ASEZHPatchSessionState.Succeeded && !apply )
				ASEZHPatchReceiptStore.Delete( projectRoot, target );
			else if( commitState != ASEZHPatchSessionState.Succeeded && apply )
				ASEZHPatchReceiptStore.Delete( projectRoot, target );
		}
		static ASEZHPatchSessionResult Rejected( string sessionId, string detail )
		{
			var result = new ASEZHPatchSessionResult
			{
				SessionId = sessionId,
				State = ASEZHPatchSessionState.PreflightRejected,
				Detail = detail
			};
			result.Results.Add( new ASEZHPatchResult
			{
				Id = "target-preflight",
				File = "Amplify Shader Editor",
				Status = "mismatch",
				Detail = detail
			} );
			return result;
		}
		static AseInstallation CreatePlanningCopy( AseInstallation source, string planRoot )
		{
			var paths = new Dictionary<string, string>( StringComparer.OrdinalIgnoreCase );
			foreach( string fileName in source.FileNames )
			{
				string destination = Path.Combine( planRoot, "sources", fileName );
				Directory.CreateDirectory( Path.GetDirectoryName( destination ) );
				File.Copy( source.AbsolutePathFor( fileName ), destination, true );
				paths[ fileName ] = destination;
			}
			string asmdef = null;
			if( !string.IsNullOrEmpty( source.AssemblyDefinitionAbsolutePath ) )
			{
				asmdef = Path.Combine( planRoot, "assembly", "AmplifyShaderEditor.asmdef" );
				Directory.CreateDirectory( Path.GetDirectoryName( asmdef ) );
				File.Copy( source.AssemblyDefinitionAbsolutePath, asmdef, true );
			}
			return AseInstallation.CreateMapped( planRoot, paths, asmdef );
		}
		static bool HasInvalidResults( List<ASEZHPatchResult> results, out string detail )
		{
			for( int i = 0; i < results.Count; i++ )
			{
				if( results[ i ].Status == "missing" || results[ i ].Status == "mismatch" )
				{
					detail = results[ i ].Id + " / " + results[ i ].File + " / " + results[ i ].Detail;
					return true;
				}
			}
			detail = null;
			return false;
		}

		static string FindResidualLocaleReference( AseInstallation installation )
		{
			foreach( string fileName in installation.FileNames )
			{
				string path = installation.AbsolutePathFor( fileName );
				string text = File.ReadAllText( path, Encoding.UTF8 );
				if( text.IndexOf( "ASELocale.", StringComparison.Ordinal ) >= 0 )
					return fileName;
			}
			return null;
		}

		static List<PatchFilePlan> BuildCommitPlan( AseInstallation target, AseInstallation planned, string backupRoot )
		{
			var pairs = new List<KeyValuePair<string, string>>();
			foreach( string fileName in target.FileNames )
				pairs.Add( new KeyValuePair<string, string>( target.AbsolutePathFor( fileName ), planned.AbsolutePathFor( fileName ) ) );
			if( !string.IsNullOrEmpty( target.AssemblyDefinitionAbsolutePath ) )
				pairs.Add( new KeyValuePair<string, string>( target.AssemblyDefinitionAbsolutePath, planned.AssemblyDefinitionAbsolutePath ) );

			var plans = new List<PatchFilePlan>();
			for( int i = 0; i < pairs.Count; i++ )
			{
				byte[] before = File.ReadAllBytes( pairs[ i ].Key );
				byte[] after = File.ReadAllBytes( pairs[ i ].Value );
				if( ByteArraysEqual( before, after ) )
					continue;
				string backup = Path.Combine( backupRoot, i.ToString( "D2" ) + "-" + Path.GetFileName( pairs[ i ].Key ) );
				File.WriteAllBytes( backup, before );
				plans.Add( new PatchFilePlan
				{
					TargetPath = pairs[ i ].Key,
					BackupPath = backup,
					Preimage = before,
					Output = after,
					PreimageHash = Sha256( before ),
					OutputHash = Sha256( after )
				} );
			}
			return plans;
		}

		static void Commit(
			List<PatchFilePlan> plans,
			int failAfterWrites,
			out ASEZHPatchSessionState state,
			out string detail )
		{
			var touched = new List<PatchFilePlan>();
			try
			{
				for( int i = 0; i < plans.Count; i++ )
				{
					PatchFilePlan plan = plans[ i ];
					if( Sha256( File.ReadAllBytes( plan.TargetPath ) ) != plan.PreimageHash )
						throw new IOException( "文件在预检后发生变化：" + plan.TargetPath );
					if( failAfterWrites >= 0 && touched.Count >= failAfterWrites )
						throw new IOException( "测试故障注入：第 " + touched.Count + " 次写入后停止" );
					ReplaceFile( plan.TargetPath, plan.Output );
					touched.Add( plan );
					if( Sha256( File.ReadAllBytes( plan.TargetPath ) ) != plan.OutputHash )
						throw new IOException( "写入后哈希不符：" + plan.TargetPath );
				}
				state = ASEZHPatchSessionState.Succeeded;
				detail = "事务提交成功，共写入 " + touched.Count + " 个文件；preimage 已保留。";
			}
			catch( Exception exception )
			{
				var failed = new List<string>();
				for( int i = touched.Count - 1; i >= 0; i-- )
				{
					try
					{
						ReplaceFile( touched[ i ].TargetPath, touched[ i ].Preimage );
						if( Sha256( File.ReadAllBytes( touched[ i ].TargetPath ) ) != touched[ i ].PreimageHash )
							failed.Add( touched[ i ].TargetPath );
					}
					catch
					{
						failed.Add( touched[ i ].TargetPath );
					}
				}
				state = failed.Count == 0
					? ASEZHPatchSessionState.FailedRestored
					: ASEZHPatchSessionState.FailedRecoveryIncomplete;
				detail = failed.Count == 0
					? "事务失败，所有已写文件已恢复：" + exception.Message
					: "事务失败且恢复不完整（" + string.Join( ", ", failed.ToArray() ) + "）：" + exception.Message;
			}
		}

		static void ReplaceFile( string targetPath, byte[] bytes )
		{
			string temporary = targetPath + ".asezh-" + Guid.NewGuid().ToString( "N" ) + ".tmp";
			try
			{
				File.WriteAllBytes( temporary, bytes );
				try
				{
					File.Replace( temporary, targetPath, null );
				}
				catch( PlatformNotSupportedException )
				{
					File.Copy( temporary, targetPath, true );
					File.Delete( temporary );
				}
			}
			finally
			{
				if( File.Exists( temporary ) )
					File.Delete( temporary );
			}
		}

		static void WriteManifest( string sessionRoot, ASEZHPatchSessionResult session, List<PatchFilePlan> plans )
		{
			var lines = new List<string>
			{
				"session=" + session.SessionId,
				"state=" + session.State,
				"target=" + session.TargetRoot
			};
			for( int i = 0; i < plans.Count; i++ )
				lines.Add( plans[ i ].TargetPath + "\t" + plans[ i ].PreimageHash + "\t" + plans[ i ].OutputHash );
			File.WriteAllLines( Path.Combine( sessionRoot, "manifest.tsv" ), lines.ToArray(), new UTF8Encoding( false ) );
		}

		static void TryWriteManifest( string sessionRoot, ASEZHPatchSessionResult session, List<PatchFilePlan> plans )
		{
			try { WriteManifest( sessionRoot, session, plans ); }
			catch( Exception exception ) { session.Detail += "；会话清单写入失败：" + exception.Message; }
		}

		internal static ASEZHPatchSessionState CommitForTests( List<PatchFilePlan> plans, int failAfterWrites, out string detail )
		{
			ASEZHPatchSessionState state;
			Commit( plans, failAfterWrites, out state, out detail );
			return state;
		}

		internal static PatchFilePlan CreatePlanForTests( string targetPath, string backupPath, byte[] output )
		{
			byte[] before = File.ReadAllBytes( targetPath );
			File.WriteAllBytes( backupPath, before );
			return new PatchFilePlan
			{
				TargetPath = targetPath,
				BackupPath = backupPath,
				Preimage = before,
				Output = output,
				PreimageHash = Sha256( before ),
				OutputHash = Sha256( output )
			};
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

		static bool ByteArraysEqual( byte[] left, byte[] right )
		{
			if( left.Length != right.Length )
				return false;
			for( int i = 0; i < left.Length; i++ )
				if( left[ i ] != right[ i ] )
					return false;
			return true;
		}

		static void TryDeleteDirectory( string path )
		{
			try
			{
				if( Directory.Exists( path ) )
					Directory.Delete( path, true );
			}
			catch
			{
				// The plan copy is not required for recovery; preimages remain outside it.
			}
		}
	}
}
