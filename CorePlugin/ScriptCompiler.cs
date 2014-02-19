﻿using System;
using System.CodeDom.Compiler;
using System.Linq;
using System.Reflection;
using Duality;
using Microsoft.CSharp;
using Mono.Cecil;
using Mono.Cecil.Pdb;

namespace ScriptingPlugin
{
	public class ScriptCompiler
	{
		public Assembly Compile(string scriptName, string scriptPath, string script)
		{
			var compilerParams = new CompilerParameters
									 {
										 GenerateInMemory = false,
										 TempFiles = new TempFileCollection(Environment.GetEnvironmentVariable("TEMP"), true),
										 IncludeDebugInformation = true,
										 TreatWarningsAsErrors = false,
										 GenerateExecutable = false,
										 CompilerOptions = " /debug:pdbonly"
									 };
			
			string[] references =
				{
					"System.dll", "System.Core.dll", "Duality.dll", "FarseerOpenTK.dll", "plugins/ScriptingPlugin.core.dll", "OpenTK.dll"
				};
			compilerParams.ReferencedAssemblies.AddRange(references);

			var provider = new CSharpCodeProvider();
			var compile = provider.CompileAssemblyFromSource(compilerParams, script);

			if (compile.Errors.HasErrors == false)
			{
				SetSourcePathInPdbFile(compile, scriptName, scriptPath);
				return compile.CompiledAssembly;
			}

			var text = compile.Errors.Cast<CompilerError>().Aggregate("", (current, ce) => current + ("rn" + ce));
			Log.Editor.WriteError("Error compiling script '{0}': {1}", scriptName, text);
			return null;
		}

		private void SetSourcePathInPdbFile(CompilerResults compile, string scriptName, string scriptPath)
		{
			var readerParameters = new ReaderParameters { ReadSymbols = true, SymbolReaderProvider = new PdbReaderProvider() };
			var assemblyDef = AssemblyDefinition.ReadAssembly(compile.PathToAssembly, readerParameters);

			var moduleDefinition = assemblyDef.Modules[0];
			moduleDefinition.ReadSymbols();
			var type = moduleDefinition.Types.FirstOrDefault(t => t.BaseType != null && t.BaseType.Name == "DualityScript");

			if (type == null)
			{
				Log.Editor.WriteError("Script file '{0}' has to contain a class that derives from DualityScript.", scriptName);
				return;
			}

			foreach (var method in type.Methods)
			{
				var instruction = method.Body.Instructions.FirstOrDefault(i => i.SequencePoint != null);
				if (instruction == null)
					continue;

				instruction.SequencePoint.Document.Url = scriptPath;
			}

			var writerParameters = new WriterParameters { WriteSymbols = true, SymbolWriterProvider = new PdbWriterProvider() };
			assemblyDef.Write(compile.PathToAssembly, writerParameters);
		}
	}
}