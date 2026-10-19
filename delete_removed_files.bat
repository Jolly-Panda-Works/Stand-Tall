@echo off
chcp 65001 >nul
rem Run from the Unity project root (the folder that contains Assets):  delete_removed_files.bat
rem No file is removed in this delivery, so the list below is empty.
rem Add one line per file:  call :del "Assets\Scripts\Example\OldFile.cs"

if not exist "Assets" (
  echo Assets folder not found. Run this from the project root.
  exit /b 1
)

rem call :del "Assets\Scripts\Example\OldFile.cs"

echo Nothing to delete.
exit /b 0

:del
if exist "%~1" (del /f /q "%~1" && echo Deleted: %~1) else echo Not found: %~1
if exist "%~1.meta" (del /f /q "%~1.meta" && echo Deleted: %~1.meta)
exit /b 0
