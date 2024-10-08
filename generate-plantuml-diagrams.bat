@echo off
setlocal EnableDelayedExpansion

REM Set the path to the PlantUML jar file
set PLANTUML_JAR_PATH=./libs/plantuml-1.2024.7.jar

REM Set the directory to search for .puml files
set SEARCH_DIR=./docs/

REM Set the output directory
set OUT_DIR=./svg/

REM Set the export format
set EXPORT_FORMAT=svg

REM Set the extra options
set EXTRA=-SdefaultFontSize=20

echo.
echo Generating files...
echo.

REM Use a for loop to find .puml files and convert them to .svg
for /r "%SEARCH_DIR%" %%F in (*.puml) do (
    echo Processing file: %%F
    set "file=%%~nF"
    java -jar "%PLANTUML_JAR_PATH%" %EXTRA% -t%EXPORT_FORMAT% "%%F" -o%OUT_DIR%
)

echo Finished

endlocal
