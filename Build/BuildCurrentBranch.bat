@echo off
setlocal
set _currentpath=%~dp0
pushd "%_currentpath%"
set _msbuildpath=C:\Windows\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe
"%_msbuildpath%" ..\LibTessDotNet.sln /t:Clean /p:Configuration=Release
"%_msbuildpath%" ..\LibTessDotNet.sln /t:Build /p:Configuration=Release

del Release\*.pdb

LibZ\libz.exe inject-dll --assembly Release\TessBed.exe --include Release\Poly2Tri.dll --include Release\nunit.framework.dll --move

copy Instructions.txt Release\Instructions.txt
copy ..\LICENSE.txt Release\MITLicense.txt

if "%1"=="" (
    set /P _version=Enter version || set _version=NONE
) else if "%appveyor_build_version%" == "" (
    set _version=%1
) else (
    set _version=%appveyor_build_version%
)
if "%_version%"=="NONE" goto :error
set _version="LibTessDotNet-%_version%"

move Release "%_version%"

Zip\zip.exe -r "%_version%.zip" "%_version%"

del /S /Q "%_version%"
rd /S /Q "%_version%"

goto :eof

:error
echo Version required
goto :eof

popd