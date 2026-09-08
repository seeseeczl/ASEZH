using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

[assembly: InternalsVisibleTo( "ASEZH.Editor.Tests" )]

namespace AmplifyShaderEditor
{
	internal sealed class AseInstallation
	{
		readonly Dictionary<string, string> m_assetPaths;
		readonly Dictionary<string, string> m_absolutePaths;

		internal string AssetRoot { get; private set; }
		internal string AbsoluteRoot { get; private set; }
		internal string AssemblyDefinitionAssetPath { get; private set; }
		internal string AssemblyDefinitionAbsolutePath { get; private set; }

		internal IEnumerable<string> FileNames { get { return m_absolutePaths.Keys; } }

		internal AseInstallation(
			string assetRoot,
			string absoluteRoot,
			Dictionary<string, string> assetPaths,
			Dictionary<string, string> absolutePaths,
			string assemblyDefinitionAssetPath,
			string assemblyDefinitionAbsolutePath )
		{
			AssetRoot = assetRoot;
			AbsoluteRoot = absoluteRoot;
			m_assetPaths = assetPaths;
			m_absolutePaths = absolutePaths;
			AssemblyDefinitionAssetPath = assemblyDefinitionAssetPath;
			AssemblyDefinitionAbsolutePath = assemblyDefinitionAbsolutePath;
		}

		internal string AssetPathFor( string fileName )
		{
			string path;
			return m_assetPaths.TryGetValue( fileName, out path ) ? path : null;
		}

		internal string AbsolutePathFor( string fileName )
		{
			string path;
			return m_absolutePaths.TryGetValue( fileName, out path ) ? path : null;
		}

		internal IEnumerable<string> AllAbsolutePaths()
		{
			foreach( string path in m_absolutePaths.Values )
				yield return path;
			if( !string.IsNullOrEmpty( AssemblyDefinitionAbsolutePath ) )
				yield return AssemblyDefinitionAbsolutePath;
		}

		internal static AseInstallation CreateMapped(
			string root,
			Dictionary<string, string> paths,
			string asmdefPath )
		{
			var asset = new Dictionary<string, string>( StringComparer.OrdinalIgnoreCase );
			var absolute = new Dictionary<string, string>( StringComparer.OrdinalIgnoreCase );
			foreach( KeyValuePair<string, string> pair in paths )
			{
				string full = Path.GetFullPath( pair.Value );
				asset[ pair.Key ] = full;
				absolute[ pair.Key ] = full;
			}
			string asmdef = string.IsNullOrEmpty( asmdefPath ) ? null : Path.GetFullPath( asmdefPath );
			return new AseInstallation( root, Path.GetFullPath( root ), asset, absolute, asmdef, asmdef );
		}
	}

	internal static class AseTargetResolver
	{
		internal static bool TryResolve( out AseInstallation installation, out string detail )
		{
			installation = null;
			List<string> requiredNames = ASEZHPatcher.Catalog()
				.Select( patch => patch.FileName )
				.Distinct( StringComparer.OrdinalIgnoreCase )
				.ToList();
			var candidates = new Dictionary<string, List<string>>( StringComparer.OrdinalIgnoreCase );
			for( int i = 0; i < requiredNames.Count; i++ )
				candidates[ requiredNames[ i ] ] = FindAssetFiles( requiredNames[ i ], "t:MonoScript" );

			Dictionary<string, Dictionary<string, string>> roots = FindCompleteRoots( requiredNames, candidates );
			if( roots.Count == 0 )
			{
				detail = "未找到包含全部受管源码的 Amplify Shader Editor 根目录。";
				return false;
			}
			if( roots.Count > 1 )
			{
				detail = "发现多个 ASE 根目录，已拒绝写入：" + string.Join( ", ", roots.Keys.ToArray() );
				return false;
			}

			if( !TryBuildInstallation( roots.First(), out installation, out detail ) )
				return false;

			string unsafeReason;
			if( !IsSafelyWritable( installation, out unsafeReason ) )
			{
				installation = null;
				detail = unsafeReason;
				return false;
			}
			detail = "已锁定唯一 ASE 根目录：" + installation.AssetRoot;
			return true;
		}

		static Dictionary<string, Dictionary<string, string>> FindCompleteRoots(
			List<string> requiredNames,
			Dictionary<string, List<string>> candidates )
		{
			List<string> undoPaths = candidates.ContainsKey( "UndoParentNode.cs" )
				? candidates[ "UndoParentNode.cs" ]
				: new List<string>();
			var roots = new Dictionary<string, Dictionary<string, string>>( StringComparer.OrdinalIgnoreCase );
			for( int i = 0; i < undoPaths.Count; i++ )
			{
				string root;
				Dictionary<string, string> files;
				if( TryFindSmallestCompleteRoot( undoPaths[ i ], requiredNames, candidates, out root, out files ) )
					roots[ root ] = files;
			}
			return roots;
		}

		static bool TryBuildInstallation(
			KeyValuePair<string, Dictionary<string, string>> selected,
			out AseInstallation installation,
			out string detail )
		{
			installation = null;
			List<string> allAsmdefs = FindAssetFiles( "AmplifyShaderEditor.asmdef", "t:AssemblyDefinitionAsset" );
			List<string> ancestorAsmdefs = allAsmdefs.Where( path => selected.Value.Values.All(
				sourcePath => IsUnder( sourcePath, NormalizeAssetPath( Path.GetDirectoryName( path ) ) ) ) ).ToList();
			string selectedRoot = ancestorAsmdefs.Count == 1
				? NormalizeAssetPath( Path.GetDirectoryName( ancestorAsmdefs[ 0 ] ) )
				: selected.Key;
			var absolute = new Dictionary<string, string>( StringComparer.OrdinalIgnoreCase );
			foreach( KeyValuePair<string, string> pair in selected.Value )
			{
				string full = ToAbsolute( pair.Value );
				if( string.IsNullOrEmpty( full ) || !File.Exists( full ) )
				{
					detail = "受管文件无法解析到磁盘：" + pair.Value;
					return false;
				}
				absolute[ pair.Key ] = full;
			}
			List<string> asmdefs = allAsmdefs.Where( path => IsUnder( path, selectedRoot ) ).ToList();
			if( asmdefs.Count > 1 )
			{
				detail = "同一 ASE 根目录存在多个 AmplifyShaderEditor.asmdef，已拒绝写入。";
				return false;
			}
			string asmdefAsset = asmdefs.Count == 1 ? asmdefs[ 0 ] : null;
			string asmdefAbsolute = string.IsNullOrEmpty( asmdefAsset ) ? null : ToAbsolute( asmdefAsset );
			string absoluteRoot = CommonAbsoluteRoot( absolute.Values.Concat(
				string.IsNullOrEmpty( asmdefAbsolute ) ? new string[ 0 ] : new[] { asmdefAbsolute } ) );
			installation = new AseInstallation( selectedRoot, absoluteRoot, selected.Value, absolute, asmdefAsset, asmdefAbsolute );
			detail = null;
			return true;
		}

		static List<string> FindAssetFiles( string fileName, string typeFilter )
		{
			string query = Path.GetFileNameWithoutExtension( fileName ) + " " + typeFilter;
			string[] guids = AssetDatabase.FindAssets( query );
			var paths = new List<string>();
			for( int i = 0; i < guids.Length; i++ )
			{
				string path = NormalizeAssetPath( AssetDatabase.GUIDToAssetPath( guids[ i ] ) );
				if( path.EndsWith( "/" + fileName, StringComparison.OrdinalIgnoreCase ) )
					paths.Add( path );
			}
			return paths.Distinct( StringComparer.OrdinalIgnoreCase ).ToList();
		}

		internal static bool TryFindSmallestCompleteRoot(
			string undoPath,
			List<string> requiredNames,
			Dictionary<string, List<string>> candidates,
			out string root,
			out Dictionary<string, string> files )
		{
			root = null;
			files = null;
			string current = NormalizeAssetPath( Path.GetDirectoryName( undoPath ) );
			while( !string.IsNullOrEmpty( current ) && current.IndexOf( '/' ) >= 0 )
			{
				var selected = new Dictionary<string, string>( StringComparer.OrdinalIgnoreCase );
				bool complete = true;
				for( int i = 0; i < requiredNames.Count; i++ )
				{
					List<string> matches = candidates[ requiredNames[ i ] ].Where( path => IsUnder( path, current ) ).ToList();
					if( matches.Count != 1 )
					{
						complete = false;
						break;
					}
					selected[ requiredNames[ i ] ] = matches[ 0 ];
				}
				if( complete )
				{
					root = current;
					files = selected;
					return true;
				}
				current = NormalizeAssetPath( Path.GetDirectoryName( current ) );
			}
			return false;
		}

		static bool IsSafelyWritable( AseInstallation installation, out string reason )
		{
			foreach( string path in installation.AllAbsolutePaths() )
			{
				string normalized = path.Replace( '\\', '/' );
				if( normalized.IndexOf( "/Library/PackageCache/", StringComparison.OrdinalIgnoreCase ) >= 0 )
				{
					reason = "ASE 位于不可持久化的 PackageCache，已拒绝写入：" + installation.AssetRoot;
					return false;
				}
				FileAttributes attributes = File.GetAttributes( path );
				if( ( attributes & FileAttributes.ReadOnly ) != 0 )
				{
					reason = "ASE 受管文件为只读，已拒绝写入：" + path;
					return false;
				}
			}
			reason = null;
			return true;
		}

		internal static string ToAbsolute( string assetPath )
		{
			if( string.IsNullOrEmpty( assetPath ) )
				return null;
			if( Path.IsPathRooted( assetPath ) )
				return Path.GetFullPath( assetPath );
			string normalized = NormalizeAssetPath( assetPath );
			if( normalized.StartsWith( "Packages/", StringComparison.OrdinalIgnoreCase ) )
			{
				UnityEditor.PackageManager.PackageInfo info = UnityEditor.PackageManager.PackageInfo.FindForAssetPath( normalized );
				if( info != null && !string.IsNullOrEmpty( info.resolvedPath ) && !string.IsNullOrEmpty( info.assetPath ) )
				{
					string suffix = normalized.Substring( info.assetPath.Length ).TrimStart( '/' );
					return Path.GetFullPath( Path.Combine( info.resolvedPath, suffix ) );
				}
			}
			string projectRoot = Path.GetDirectoryName( Application.dataPath );
			return Path.GetFullPath( Path.Combine( projectRoot, normalized ) );
		}

		static string CommonAbsoluteRoot( IEnumerable<string> paths )
		{
			string[] all = paths.Select( Path.GetFullPath ).ToArray();
			if( all.Length == 0 )
				return null;
			string common = Path.GetDirectoryName( all[ 0 ] );
			while( !string.IsNullOrEmpty( common ) && !all.All( path => IsUnderAbsolute( path, common ) ) )
				common = Path.GetDirectoryName( common );
			return common;
		}

		static bool IsUnder( string path, string root )
		{
			string normalizedPath = NormalizeAssetPath( path );
			string normalizedRoot = NormalizeAssetPath( root ).TrimEnd( '/' );
			return normalizedPath.Equals( normalizedRoot, StringComparison.OrdinalIgnoreCase )
				|| normalizedPath.StartsWith( normalizedRoot + "/", StringComparison.OrdinalIgnoreCase );
		}

		static bool IsUnderAbsolute( string path, string root )
		{
			string normalizedPath = Path.GetFullPath( path ).TrimEnd( Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar );
			string normalizedRoot = Path.GetFullPath( root ).TrimEnd( Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar );
			return normalizedPath.Equals( normalizedRoot, StringComparison.OrdinalIgnoreCase )
				|| normalizedPath.StartsWith( normalizedRoot + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase );
		}

		static string NormalizeAssetPath( string path )
		{
			return string.IsNullOrEmpty( path ) ? string.Empty : path.Replace( '\\', '/' ).TrimEnd( '/' );
		}
	}
}
