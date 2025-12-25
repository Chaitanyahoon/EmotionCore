@echo off
setlocal
set PROJECT_PATH=%~dp0
:: Remove trailing backslash
if %PROJECT_PATH:~-1%==\ set PROJECT_PATH=%PROJECT_PATH:~0,-1%

echo Detecting Unity Installation...

:: Use PowerShell to find the first Unity.exe in the Hub/Editor folder
for /f "delims=" %%i in ('powershell -Command "Get-ChildItem -Path 'C:\Program Files\Unity\Hub\Editor' -Filter Unity.exe -Recurse | Select-Object -First 1 -ExpandProperty FullName"') do set UNITY_PATH="%%i"

if [%UNITY_PATH%] == [] (
    echo Could not find Unity.exe in C:\Program Files\Unity\Hub\Editor.
    echo Please edit this script and set UNITY_PATH manually.
    pause
    exit /b
)

echo Found Unity at: %UNITY_PATH%

echo.
echo ===================================================
echo STEP 1: GENERATING WORLD (Assets, Geometry, Logic)
echo ===================================================
echo ===================================================
%UNITY_PATH% -quit -batchmode -projectPath "%PROJECT_PATH%" -executeMethod EmotionCore.EditorTools.WorldGenerator.ConstructWorld -logFile build_gen.log
if %ERRORLEVEL% NEQ 0 (
    echo World Generation Failed! Check build_gen.log
    pause
    exit /b
)

echo.
echo ===================================================
echo STEP 2: BUILDING EXECUTABLE
echo ===================================================
echo ===================================================
%UNITY_PATH% -quit -batchmode -projectPath "%PROJECT_PATH%" -executeMethod EmotionCore.EditorTools.AutoBuilder.BuildProject -logFile build_exe.log
if %ERRORLEVEL% NEQ 0 (
    echo Build Failed! Check build_exe.log
    pause
    exit /b
)

echo.
echo ===================================================
echo BUILD COMPLETE
echo ===================================================
echo Executable is in the 'Builds' folder.
pause
