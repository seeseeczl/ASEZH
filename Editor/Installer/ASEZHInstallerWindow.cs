using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AmplifyShaderEditor
{
	public class ASEZHInstallerWindow : EditorWindow
	{
		Vector2 m_scroll;
		List<ASEZHPatchResult> m_results;

		[MenuItem( "Window/ASEZH/接入 Amplify Shader Editor", false, 2090 )]
		static void Open()
		{
			GetWindow<ASEZHInstallerWindow>( true, "ASEZH 接入" );
		}

		[MenuItem( "Window/ASEZH/移除汉化补丁", false, 2091 )]
		static void RemoveMenu()
		{
			if( !EditorUtility.DisplayDialog( "ASEZH", "将从 ASE 源码中移除 ASEZH 显示钩子。ASE 本体和本包都会保留，可稍后再次接入。", "移除", "取消" ) )
				return;
			List<ASEZHPatchResult> results = ASEZHPatcher.RemoveAll();
			int n = 0;
			for( int i = 0; i < results.Count; i++ )
			{
				if( results[ i ].Status == "removed" )
					n++;
			}
			EditorUtility.DisplayDialog( "ASEZH", "已撤回 " + n + " 处钩子。请等编译结束后再打开 ASE。", "确定" );
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
			EditorGUILayout.LabelField( "ASE 根目录", string.IsNullOrEmpty( root ) ? "未找到 UndoParentNode.cs" : root );

			EditorGUILayout.Space();
			if( GUILayout.Button( "重新扫描", GUILayout.Height( 24 ) ) )
				m_results = ASEZHPatcher.Scan();
			if( GUILayout.Button( "应用可自动补丁", GUILayout.Height( 28 ) ) )
			{
				if( EditorUtility.DisplayDialog( "ASEZH", "将改写 ASE 源码中的锚点片段。已打过的补丁会跳过。建议先提交或备份 ASE。", "应用", "取消" ) )
					m_results = ASEZHPatcher.ApplyAll();
			}
			if( GUILayout.Button( "移除汉化补丁", GUILayout.Height( 24 ) ) )
			{
				if( EditorUtility.DisplayDialog( "ASEZH", "将从 ASE 源码中移除 ASEZH 显示钩子。ASE 本体和本包都会保留，可稍后再次接入。", "移除", "取消" ) )
					m_results = ASEZHPatcher.RemoveAll();
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
