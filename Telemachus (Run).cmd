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
CD /D C:\Telemachus
Telemachus.exe
IF %ERRORLEVEL% NEQ 0 (
    PAUSE
    EXIT /B %ERRORLEVEL%
)
