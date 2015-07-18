using System;
using System.IO;
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

		protected override void InitPlugin()
		{
			base.InitPlugin();
#if !__ANDROID__
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
}