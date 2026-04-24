@ECHO OFF
SET ASPNETCORE_ENVIRONMENT=Vessel
WHERE dotnet >nul 2>nul
IF %ERRORLEVEL% NEQ 0 (
    ECHO dotnet command not found. Please download the .NET Runtime ^(not the SDK^) from the following link:
    ECHO https://dotnet.microsoft.com/en-us/download/dotnet/latest
    exit %ERRORLEVEL%
)
TASKLIST | FIND /I "Telemachus.exe" 2>nul
IF %ERRORLEVEL% EQU 0 (
    TASKKILL /IM Telemachus.exe /F 2>nul
    timeout /T 3 /NOBREAK 2>nul
)
TASKLIST | FIND /I "Telemachus.exe" 2>nul
IF %ERRORLEVEL% EQU 0 (
    ECHO Telemachus.exe is still running. Exiting with error.
    EXIT /B 1
)
if exist "Telemachus-x64.zip" (
    del /q "Telemachus-x64.zip"
)
ECHO Please wait...
powershell Invoke-WebRequest -Uri http://85.72.41.81:3000/download/Telemachus-x64.zip -OutFile Telemachus-x64.zip
powershell Expand-Archive -Path Telemachus-x64.zip -DestinationPath "C:\Telemachus" -Force
if not exist "C:\Telemachus\appsettings.json" (
    if exist "appsettings.json" (
        copy "appsettings.json" "C:\Telemachus\"
    )
)
CD /D C:\Telemachus
Telemachus.exe
IF %ERRORLEVEL% NEQ 0 (
    PAUSE
    EXIT /B %ERRORLEVEL%
)
