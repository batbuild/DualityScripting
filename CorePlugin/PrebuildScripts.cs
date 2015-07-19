using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Duality;
#if __ANDROID__
using Duality.Utility;
#endif
namespace ScriptingPlugin
{
	public class PrebuildScripts
	{
		private static List<Assembly> _resultingAssemblies;

		public static Assembly[] LoadAssemblies()
		{
			_resultingAssemblies = _resultingAssemblies ?? new List<Assembly>();
			if (_resultingAssemblies.Count != 0) 
				return _resultingAssemblies.ToArray();
			IEnumerable<FileInfo> scriptsDll;
			
#if __ANDROID__
			var scriptsPath = "Scripts";
			var scripts = ContentProvider.AndroidAssetManager.List(scriptsPath);
			var dlls = scripts.Where(x => x.EndsWith(".dll"));
			scriptsDll = dlls.Select(dll => new FileInfo(scriptsPath+"/"+ dll));
#else
			var scriptsDirectory = new DirectoryInfo("Scripts");
			if (!scriptsDirectory.Exists)
				return new Assembly[]{};
			scriptsDll = scriptsDirectory.GetFiles("*.dll", SearchOption.TopDirectoryOnly);
#endif
			
			foreach (var scriptAssembly in scriptsDll)
			{
				string scriptPath;
				Assembly assembly;
#if __ANDROID__
				scriptPath = scriptAssembly.FullName.Remove(0,1);
				if (!FileHelper.FileExists(scriptPath ))
					continue;
				assembly = Assembly.LoadFrom(AndroidAssetsHelper.CalculatePathToDisk(scriptPath));
#else
				scriptPath = scriptAssembly.FullName;
				if (!scriptAssembly.Exists)
					continue;
				assembly = Assembly.LoadFrom(Path.GetFullPath(scriptPath));
#endif
					
				_resultingAssemblies.Add(assembly);
				Log.Editor.Write("Loading script assembly {0} from Scripts directory", assembly.FullName);
			}

			return _resultingAssemblies.ToArray();
		}
	}

}