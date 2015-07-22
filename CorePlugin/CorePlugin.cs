using System;
using System.IO;
using System.Linq;
#if !__ANDROID__
using System.IO.Abstractions;
#endif
using Duality;

namespace ScriptingPlugin
{
	public class ScriptingPluginCorePlugin : CorePlugin
	{
		public const string DataScripts = "Data\\Scripts\\";
		public const string ReferenceAssembliesFile = "ScriptReferences.txt";

		public static IScriptMetadataService ScriptMetadataService { get; set; }
		public static IScriptCompilerService CSharpScriptCompiler { get; set; }
		public static IScriptCompilerService FSharpScriptCompiler { get; set; }

		

		protected override void InitPlugin()
		{
			base.InitPlugin();
			
#if __ANDROID__
			var scriptsPath = "Scripts";
			var scripts = ContentProvider.AndroidAssetManager.List(scriptsPath);
			var dlls = scripts.Where(x => x.EndsWith(".dll"));
			foreach (var scriptAssembly in dlls)
			{
				var scriptPath = AndroidAssetsHelper.SaveToDisk(scriptsPath, scriptAssembly);
				Log.Game.Write("Saved script {0} to {1}", scriptAssembly, scriptPath);
			}
#else
			ScriptMetadataService = new ScriptMetadataService(new FileSystem());
#endif
		}

		public static void ExcludeAssembliesFromTypeSearch()
		{
			ReflectionHelper.ExcludeFromTypeSearches(new[]
			{
				"FSharp.Compiler.Service",
				"Microsoft.CodeAnalysis.CSharp",
				"Microsoft.CodeAnalysis",
				"System.Reflection.Metadata",
				"System.Collections.Immutable",
				"Mono.Cecil",
				"Mono.Cecil.Pdb"
			});
		}
	}

	public static class AndroidAssetsHelper
	{
#if __ANDROID__
		public static string CalculatePathToDisk(string assetName)
		{
			if (Path.IsPathRooted(assetName))
			{
				Console.WriteLine("Invalid filename. Fmod assets requires a relative path.");
				return "";
			}
			string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			return Path.Combine(path, assetName);
		}

		public static string SaveToDisk(string assetBasePath, string assetBaseName)
		{
			assetBasePath = assetBasePath.TrimEnd(new []{'\\'});
			var assetName = GetFilename(Path.Combine(assetBasePath, assetBaseName));
			string assetPath = CalculatePathToDisk(assetName);

			var dir = Path.GetDirectoryName(assetPath);
			if (Directory.Exists(dir) == false)
				Directory.CreateDirectory(dir);
			
			try
			{
				using (var stream = Duality.ContentProvider.AndroidAssetManager.Open(assetName, Android.Content.Res.Access.Streaming))
				using (var streamWriter = new FileStream(assetPath, System.IO.FileMode.Create))
				{
					stream.CopyTo(streamWriter);
					streamWriter.Flush();
				}
			}
			catch (Exception e)
			{
				Log.Core.WriteError("An error ocurred when converting asset to a file on the device. E {0} {1}", e.Message, e.StackTrace);
			}
			return assetPath;
		}

		internal static string GetFilename(string name)
		{
			const char forwardSlash = '/';
			const char backSlash = '\\';
			var notSeparator = Path.DirectorySeparatorChar == backSlash ? forwardSlash : backSlash;
			var nameUri = new Uri("file:///" + name).LocalPath.Substring(1);
			return nameUri.Replace(notSeparator, Path.DirectorySeparatorChar);
		}


#endif
	}

}