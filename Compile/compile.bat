@echo off

::Use this in case, you haven't VS installed, or you wan't to open it!
::Maybe you need to install this, if you don't have VS 2015 or MSBuild packages: https://www.microsoft.com/en-us/download/details.aspx?id=48159

if "%MAIN_PATH%" == "" ( for %%a in ("%~dp0..") do set "MAIN_PATH=%%~fa" )
if "%REF_PATH%" == "" ( set "REF_PATH=%MAIN_PATH%\Assemblies" )

set MSBuildEmitSolution=1
"%~dp0\msbuild.exe" "%MAIN_PATH%\PCStats.sln" /pp /p:Platform="Any CPU" /p:OutputPath=../Build /p:Configuration=Debug

pause