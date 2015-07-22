using System;
using Duality;
#if !__ANDROID__
using Duality.Editor;
#endif

namespace ScriptingPlugin.Resources
{
	[Serializable]
#if !__ANDROID__
	[EditorHintCategory("Scripting")]
	[EditorHintImage("Resources", "csharp")]
#endif
	public class CSharpScript : ScriptResourceBase
	{
		public new static string FileExt = ".CSharpScript" + Resource.FileExt;

		public CSharpScript()
		{
		}

		public CSharpScript(IScriptCompilerService compilerService)
		{
			ScriptCompiler = compilerService;
		}

		protected override void OnLoaded()
		{
			base.OnLoaded();

			ScriptCompiler = ScriptingPluginCorePlugin.CSharpScriptCompiler;

		}
	}
}