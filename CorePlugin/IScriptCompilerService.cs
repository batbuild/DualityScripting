namespace ScriptingPlugin
{
	public interface IScriptCompilerService
	{
		ScriptCompilerResult TryCompile(string scriptName, string scriptPath, string script);
#if !__ANDROID__
		void SetPdbEditor(IPdbEditor pdbEditor);
#endif
	}
}