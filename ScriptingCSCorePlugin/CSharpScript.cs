using System;
using Duality;
#if !__ANDROID__
using Duality.Editor;
using ScriptingPlugin.CSharp;
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
#if DEBUG
			ScriptCompiler = ScriptingPluginCorePlugin.CSharpScriptCompiler;
#endif
			base.OnLoaded();
		}
	}
}