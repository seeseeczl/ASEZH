using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AmplifyShaderEditor
{
	public class ASEZHInstallerWindow : EditorWindow
	{
		Vector2 m_scroll;
		List<ASEZHPatchResult> m_results;
		bool m_showDiagnostics;

		[MenuItem( "Window/ASEZH/接入 Amplify Shader Editor", false, 2090 )]
		static void Open()
		{
			GetWindow<ASEZHInstallerWindow>( true, "ASEZH 接入" );
		}

		static void ShowSessionDialog( string operation )
		{
			ASEZHPatchSessionResult session = ASEZHPatcher.LastSession;
			if( session == null )
			{
				EditorUtility.DisplayDialog( "ASEZH", operation + "没有产生可核验结果。", "确定" );
				return;
			}
			string target = string.IsNullOrEmpty( session.TargetRoot ) ? "未锁定" : session.TargetRoot;
			string backup = string.IsNullOrEmpty( session.BackupPath ) ? "未创建" : session.BackupPath;
			string summary = "状态：" + session.State
				+ "\n目标：" + target
				+ "\n备份：" + backup
				+ "\n结论：" + session.Detail;
			if( session.IsSuccess )
				summary += "\n\n请等编译结束后再打开 ASE。";
			EditorUtility.DisplayDialog( "ASEZH " + operation, summary, "确定" );
		}

		void OnEnable()
		{
			m_results = ASEZHPatcher.Scan();
		}

		void OnGUI()
		{
			EditorGUILayout.LabelField( "把 ASEZH 显示层接到当前工程里的 Amplify Shader Editor。", EditorStyles.wordWrappedLabel );
			EditorGUILayout.Space();
			string root = ASEZHPatcher.FindAseRoot();
			EditorGUILayout.LabelField( "ASE 根目录", string.IsNullOrEmpty( root ) ? "未找到唯一且可安全写入的 ASE" : root );
			ASEZHPatchSessionResult session = ASEZHPatcher.LastSession;
			if( session != null && !string.IsNullOrEmpty( session.Detail ) )
			{
				MessageType type = session.State == ASEZHPatchSessionState.PreflightRejected
					|| session.State == ASEZHPatchSessionState.FailedRestored
					|| session.State == ASEZHPatchSessionState.FailedRecoveryIncomplete
					? MessageType.Warning
					: MessageType.Info;
				EditorGUILayout.HelpBox( "会话状态：" + session.State + "\n" + session.Detail, type );
			}

			EditorGUILayout.Space();
			if( GUILayout.Button( "重新扫描", GUILayout.Height( 24 ) ) )
				m_results = ASEZHPatcher.Scan();
			if( GUILayout.Button( "应用可自动补丁", GUILayout.Height( 28 ) ) )
			{
				if( EditorUtility.DisplayDialog( "ASEZH", "将改写 ASE 源码中的锚点片段。已打过的补丁会跳过。建议先提交或备份 ASE。", "应用", "取消" ) )
				{
					m_results = ASEZHPatcher.ApplyAll();
					ShowSessionDialog( "应用" );
				}
			}
			if( GUILayout.Button( "移除汉化补丁", GUILayout.Height( 24 ) ) )
			{
				if( EditorUtility.DisplayDialog( "ASEZH", "将从 ASE 源码中移除 ASEZH 显示钩子。ASE 本体和本包都会保留，可稍后再次接入。", "移除", "取消" ) )
				{
					m_results = ASEZHPatcher.RemoveAll();
					ShowSessionDialog( "撤回" );
				}
			}

			EditorGUILayout.Space();
			m_showDiagnostics = EditorGUILayout.Foldout( m_showDiagnostics, "高级/诊断" );
			if( m_showDiagnostics )
			{
				EditorGUILayout.BeginVertical( EditorStyles.helpBox );
				if( GUILayout.Button( "重新加载词典", GUILayout.Height( 24 ) ) )
				{
					ASELocale.Reload();
					string error = ASELocale.RunSelfTests();
					EditorUtility.DisplayDialog( "ASEZH 重新加载词典",
						string.IsNullOrEmpty( error ) ? "已重新加载 " + ASELocale.EntryCount + " 条词典并通过本地化测试。" : error,
						"确定" );
				}
				if( GUILayout.Button( "运行本地化测试", GUILayout.Height( 24 ) ) )
				{
					string error = ASELocale.RunSelfTests();
					EditorUtility.DisplayDialog( "ASEZH 本地化测试", string.IsNullOrEmpty( error ) ? "本地化测试通过。" : error, "确定" );
				}
				EditorGUILayout.EndVertical();
			}

			EditorGUILayout.Space();
			m_scroll = EditorGUILayout.BeginScrollView( m_scroll );
			if( m_results != null )
			{
				for( int i = 0; i < m_results.Count; i++ )
				{
					var r = m_results[ i ];
					Color old = GUI.color;
					if( r.Status == "applied" || r.Status == "patched" )
						GUI.color = new Color( 0.6f, 1f, 0.6f );
					else if( r.Status == "ready" || r.Status == "removed" )
						GUI.color = new Color( 1f, 0.92f, 0.5f );
					else
						GUI.color = new Color( 1f, 0.7f, 0.7f );
					EditorGUILayout.BeginVertical( EditorStyles.helpBox );
					GUI.color = old;
					EditorGUILayout.LabelField( r.Id + "  [" + r.Status + "]" );
					EditorGUILayout.LabelField( r.File, EditorStyles.miniLabel );
					EditorGUILayout.LabelField( r.Detail, EditorStyles.wordWrappedMiniLabel );
					EditorGUILayout.EndVertical();
				}
			}
			EditorGUILayout.EndScrollView();

			EditorGUILayout.HelpBox(
				"mismatch 表示当前 ASE 源码与内置锚点的方法签名仍对不上。安装器已忽略 tab/空格差异。对照 docs/hook-sites.md 手工接入即可，不要强行套用补丁。",
				MessageType.Info );
		}
	}
}
