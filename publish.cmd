@echo off
git fetch origin main
git checkout main Telemachus.Web/package-lock.json
git pull origin main
rename "C:\inetpub\telemachus\app__offline.htm" app_offline.htm
if not exist "C:\inetpub\telemachus\app_offline.htm" (
    echo Cannot initiate file app_offline.htm!
    goto :error
)
%windir%\system32\inetsrv\appcmd recycle apppool /apppool.name:"telemachus"
dotnet publish Telemachus.Api\Telemachus -c Release -r win-x64 --self-contained false --framework netcoreapp3.1 --force -o C:\inetpub\telemachus
IF %ERRORLEVEL% NEQ 0 (
    goto :error
)
rename "C:\inetpub\telemachus\app_offline.htm" app__offline.htm
if %errorlevel% neq 0 (
    echo Cannot rename file app_offline.htm!
    goto :error
)
:error
pause
exit /b %errorlevel%
