// include Fake lib
#r @"packages\FAKE\tools\FakeLib.dll"

open Fake
open Fake.AssemblyInfoFile

// Directories
let buildDir  = @".\build\"
let testDir   = @".\test\"
let packagesDir = @".\deploy\"
// version info
let version = "0.2.0-beta"  // or retrieve from CI server

RestorePackages()
// Targets
Target "Clean" (fun _ ->
    CleanDirs [buildDir; testDir; packagesDir] 
)

Target "SetVersions" (fun _ ->
    CreateCSharpAssemblyInfo "./CorePlugin/Properties/AssemblyInfo.cs"
        [Attribute.Title "Duality Scripting"
         Attribute.Description "Scripting for Duality"         
         Attribute.Product "DualityScripting"
         Attribute.Version version
         Attribute.FileVersion version]
    CreateCSharpAssemblyInfo "./EditorPlugin/Properties/AssemblyInfo.cs"
        [Attribute.Title "Duality Scripting Editor"
         Attribute.Description "Scripting for Duality. Editing assemblies"         
         Attribute.Product "DualityScriptingEditor"
         Attribute.Version version
         Attribute.FileVersion version]
    CreateCSharpAssemblyInfo "./ScriptingCSCorePlugin/Properties/AssemblyInfo.cs"
        [Attribute.Title "Duality Scripting C#"
         Attribute.Description "Scripting for Duality C# dependencies"         
         Attribute.Product "DualityScriptingCSharp"
         Attribute.Version version
         Attribute.FileVersion version]
    CreateCSharpAssemblyInfo "./ScriptingFSCorePlugin/Properties/AssemblyInfo.cs"
        [Attribute.Title "Duality Scripting F#"
         Attribute.Description "Scripting for Duality f# dependencies"         
         Attribute.Product "DualityScriptingFSharp"
         Attribute.Version version
         Attribute.FileVersion version]
)

Target "RestorePackages" (fun _ ->
    !! "./**/packages.config"
        |> Seq.iter (RestorePackage (fun p ->
            { p with Sources = ["https://www.myget.org/F/6416d9912a7c4d46bc983870fb440d25/"]}))
)

Target "Build" (fun _ ->          
    let buildMode = getBuildParamOrDefault "buildMode" "Release"
    let setParams defaults =
        { defaults with
            Verbosity = Some(Normal)            
            Properties =
                [            
                    "Configuration", buildMode                    
                ]
        }
    build setParams "./DualityScriptingPlugins.sln"    
    |> DoNothing  
)

Target "BuildTest" (fun _ ->
    !! "**/*.Test*.*sproj"
      |> MSBuildDebug testDir "Build"
      |> Log "TestBuild-Output: "
)

Target "NUnitTest" (fun _ ->         
     !! (testDir + @"\*Test*.*.dll") 
      |> NUnit (fun p ->
                 {p with
                   DisableShadowCopy = true;
                   OutputFile = testDir + @"TestResults.xml"})
)

Target "CreateNuget" (fun _ ->      
    ["nuget/ScriptingPlugin.nuspec";
    "nuget/ScriptingPlugin.CSharp.nuspec";
    "nuget/ScriptingPlugin.Editor.nuspec";
    "nuget/ScriptingPlugin.FSharp.nuspec"]
    |> List.iter (fun spec ->
    NuGet (fun p -> 
        {p with 
            Version = version                        
            PublishUrl = getBuildParamOrDefault "nugetrepo" ""
            AccessKey = getBuildParamOrDefault "keyfornuget" ""
            Publish = hasBuildParam "nugetrepo"
            OutputPath = packagesDir
        }) spec)
)

// Dependencies
"Clean"    
  ==> "SetVersions"
  ==> "RestorePackages"
  ==> "Build"
  ==> "BuildTest"
  ==> "NUnitTest"  
  ==> "CreateNuget"  

// start build
RunTargetOrDefault "CreateNuget"