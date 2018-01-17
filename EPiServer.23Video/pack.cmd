@echo off
setlocal
SET CurrentDir=%~dp0
set version=%1
if [%1] == [] set version=11.0.2
"D:\dev\Horingssvar\src\packages\OctoPack.3.0.31\tools\NuGet.exe" pack "%CurrentDir%EPiCode.23Video.Core.nuspec" -Version %version%

