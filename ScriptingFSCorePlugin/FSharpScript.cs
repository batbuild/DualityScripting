using System;
using Duality;
#if !__ANDROID__
using Duality.Editor;
using ScriptingPlugin.FSharp;
#endif
namespace ScriptingPlugin.Resources
{
	[Serializable]
#if !__ANDROID__
	[EditorHintCategory("Scripting")]
	[EditorHintImage("Resources", "fsharp")]
#endif
	public class FSharpScript : ScriptResourceBase
	{
		public new static string FileExt = ".FSharpScript" + Resource.FileExt;

		protected override void OnLoaded()
		{
#if DEBUG
			ScriptCompiler = ScriptingPluginCorePlugin.FSharpScriptCompiler;
#endif
			base.OnLoaded();
		}
	}
}