@echo off
cls
if not exist packages\FAKE\tools\Fake.exe (
	".nuget\NuGet.exe" "Install" "FAKE" "-OutputDirectory" "packages" "-ExcludeVersion"
)
"packages\FAKE\tools\Fake.exe" CompleteBuild.fsx %*
