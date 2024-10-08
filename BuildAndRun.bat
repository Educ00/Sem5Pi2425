@echo off
:: Build the project
echo Building the project...
dotnet build

:: Check if the build was successful
if %ERRORLEVEL% NEQ 0 (
    echo Build failed!
    pause
    exit /b %ERRORLEVEL%
)

:: Run the project
echo Running the project...
dotnet run
