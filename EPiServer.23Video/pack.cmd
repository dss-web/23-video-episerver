@echo off
setlocal
SET CurrentDir=%~dp0
set version=%1
if [%1] == [] set version=3.0.5
"C:\dev\GitHub\BVNetwork Newsletter\src\.nuget\NuGet.exe" pack "%CurrentDir%EPiCode.23Video.nuspec" -Version %version%

