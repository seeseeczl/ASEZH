using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace AmplifyShaderEditor
{
	/// <summary>Display-only adapter. This assembly must not reference ASE types.</summary>
	public static class ASENativeDisplay
	{
		sealed class Metadata
		{
			internal string Name;
			internal string Category;
			internal bool Native;
		}
		sealed class DisplayState { internal int Revision = -1; }
		static readonly Dictionary<Type, Metadata> Types = new Dictionary<Type, Metadata>();
		static readonly ConditionalWeakTable<object, DisplayState> States = new ConditionalWeakTable<object, DisplayState>();
		static int s_revision;
		static readonly string[] IndexedPrefixes = { "In", "Key", "Layer" };
		public static void Invalidate() { s_revision++; }

		static Metadata Describe( Type type )
		{
			if( type == null ) return new Metadata();
			Metadata result;
			if( Types.TryGetValue( type, out result ) ) return result;
			result = new Metadata();
			try
			{
				foreach( object attribute in type.GetCustomAttributes( false ) )
				{
					Type attributeType = attribute.GetType();
					if( attributeType.FullName != "AmplifyShaderEditor.NodeAttributes" ) continue;
					result.Name = attributeType.GetField( "Name" ).GetValue( attribute ) as string;
					result.Category = attributeType.GetField( "Category" ).GetValue( attribute ) as string;
					break;
				}
				result.Native = IsNative( type.FullName, result.Category );
			}
			catch( Exception ) { result.Native = false; }
			Types[ type ] = result;
			return result;
		}

		public static bool IsNative( string typeName, string category )
		{
			return typeName != null && !string.IsNullOrEmpty( category )
				&& category.IndexOf( "flyme", StringComparison.OrdinalIgnoreCase ) < 0
				&& ASENativeTypes.Names.Contains( typeName )
				&& typeName != "AmplifyShaderEditor.FunctionNode";
		}

		public static string Title( object node, string original )
		{
			if( node == null || !ASELocale.UseChinese ) return original;
			Metadata metadata = Describe( node.GetType() );
			// Renamed properties, custom expression titles and function input names are user data.
			if( !metadata.Native || string.IsNullOrEmpty( original ) ) return original;
			if( original == metadata.Name ) return ASELocale.T( original, ASELocale.TableNodeTitle );
			string prefix = metadata.Name;
			if( node.GetType().Name == "Vector2Node" || node.GetType().Name == "Vector3Node" || node.GetType().Name == "Vector4Node" ) prefix = "Vector";
			if( !string.IsNullOrEmpty( prefix ) && original.StartsWith( prefix + " ", StringComparison.Ordinal ) )
			{
				string suffix = original.Substring( prefix.Length + 1 );
				bool digits = suffix.Length > 0;
				foreach( char c in suffix ) digits &= c >= '0' && c <= '9';
				if( digits ) return ASELocale.T( prefix, ASELocale.TableNodeTitle ) + " " + suffix;
			}
			return original;
		}

		public static GUIContent TitleContent( object node, GUIContent original )
		{
			return original == null ? null : new GUIContent( Title( node, original.text ), original.image, original.tooltip );
		}

		public static string ListLabel( Type type, string category, string original, string withShortcut )
		{
			if( type == null || !IsNative( type.FullName, category ) ) return withShortcut ?? original;
			return ASELocale.TNodeListLabel( original, withShortcut );
		}

		public static string Port( object node, bool input, int portId, string original, bool editable )
		{
			if( node == null || editable || !ASELocale.UseChinese || string.IsNullOrEmpty( original ) ) return original;
			Type type = node.GetType();
			Metadata metadata = Describe( type );
			if( !metadata.Native || type.Name == "CustomExpressionNode" ) return original;
			string context = type.FullName + "/" + ( input ? "in" : "out" ) + "/" + portId + "/" + original;
			string translated;
			if( ASELocaleStore.TryLookup( context, "port_label", out translated ) ) return translated;
			translated = ASELocale.T( original, "port_label" );
			if( translated != original ) return translated;
			foreach( string prefix in IndexedPrefixes )
			{
				if( !original.StartsWith( prefix, StringComparison.Ordinal ) ) continue;
				string suffix = original.Substring( prefix.Length ).TrimStart();
				bool digits = suffix.Length > 0;
				foreach( char c in suffix ) digits &= c >= '0' && c <= '9';
				if( digits ) return ASELocale.T( prefix, "port_label" ) + " " + suffix;
			}
			return original;
		}

		/// <summary>Only tells the ASE drawing path to recalculate geometry. Never calls OnNodeChange.</summary>
		public static bool NeedsLayout( object node )
		{
			if( node == null || !Describe( node.GetType() ).Native ) return false;
			DisplayState state = States.GetValue( node, key => new DisplayState() );
			if( state.Revision == s_revision ) return false;
			state.Revision = s_revision;
			return true;
		}
	}
}
