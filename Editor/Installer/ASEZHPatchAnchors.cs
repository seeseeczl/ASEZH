using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AmplifyShaderEditor
{
	/// <summary>
	/// 一个钩子可以有同一语义的多套锚点。ASE 版本漂移若只改方法签名，安装器仍能命中，
	/// 并把源码里真实的那一行写回替换结果，避免把旧签名盖回新版本。
	/// </summary>
	internal static class ASEZHPatchAnchors
	{
		internal static List<KeyValuePair<string, string>> Variants( ASEZHPatch patch )
		{
			var variants = new List<KeyValuePair<string, string>>();
			if( !string.IsNullOrEmpty( patch.Find ) )
				variants.Add( new KeyValuePair<string, string>( patch.Find, patch.Replace ) );
			if( !string.IsNullOrEmpty( patch.LegacyReplace ) )
				variants.Add( new KeyValuePair<string, string>( patch.LegacyReplace, patch.Replace ) );
			if( !string.IsNullOrEmpty( patch.AltFind ) )
				variants.Add( new KeyValuePair<string, string>( patch.AltFind, patch.AltReplace ) );
			return variants;
		}

		internal static bool IsApplied( ASEZHPatch patch, string text )
		{
			bool marked = !string.IsNullOrEmpty( patch.Marker ) && ASEZHPatchTransforms.ContainsFlexible( text, patch.Marker );
			foreach( KeyValuePair<string, string> candidate in Variants( patch ) )
				if( !string.IsNullOrEmpty( candidate.Value ) && ASEZHPatchTransforms.ContainsFlexible( text, candidate.Value ) )
					marked = true;
			if( !marked )
				return false;
			if( patch.ReplaceAll )
			{
				foreach( KeyValuePair<string, string> variant in Variants( patch ) )
					if( ASEZHPatchTransforms.ContainsFlexible( text, variant.Key ) )
						return false;
			}
			return true;
		}

		internal static bool IsRemoved( ASEZHPatch patch, string text )
		{
			// 接入后锚点原文可能仍在（守卫块插在锚点后面），所以先看替换文本是否还在。
			foreach( KeyValuePair<string, string> variant in Variants( patch ) )
				if( !string.IsNullOrEmpty( variant.Value ) && ASEZHPatchTransforms.ContainsFlexible( text, variant.Value ) )
					return false;
			if( !string.IsNullOrEmpty( patch.Marker ) && ASEZHPatchTransforms.ContainsFlexible( text, patch.Marker ) )
				return false;
			foreach( KeyValuePair<string, string> variant in Variants( patch ) )
				if( ASEZHPatchTransforms.ContainsFlexible( text, variant.Key ) )
					return true;
			return false;
		}

		internal static bool TryMatch( ASEZHPatch patch, string text, out Match match, out string replace )
		{
			foreach( KeyValuePair<string, string> variant in Variants( patch ) )
			{
				if( ASEZHPatchTransforms.TryMatchFlexible( text, variant.Key, out match ) )
				{
					replace = variant.Value;
					return true;
				}
			}
			match = Match.Empty;
			replace = null;
			return false;
		}

		internal static string ReverseVariant( string text, string replace, string find, bool replaceAll )
		{
			string next = text;
			Match match;
			while( ASEZHPatchTransforms.TryMatchFlexible( next, replace, out match ) )
			{
				string adapted = ASEZHPatchTransforms.AdaptStyle( find, match.Value );
				string updated = next.Remove( match.Index, match.Length ).Insert( match.Index, adapted );
				if( updated == next || !replaceAll )
				{
					next = updated;
					break;
				}
				next = updated;
			}
			return next;
		}
	}
}
