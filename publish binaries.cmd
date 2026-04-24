@echo off
git fetch origin
git checkout main Telemachus.Web/package-lock.json
git pull
dotnet publish Telemachus.Api\Telemachus -c Release -r win-x64 -p:PublishSingleFile=true --self-contained false --framework netcoreapp3.1 --force -o bin
IF %ERRORLEVEL% NEQ 0 (
    goto :error
)
if exist "Telemachus.Web\public\download\Telemachus-x64.zip" (
    del /q "Telemachus.Web\public\download\Telemachus-x64.zip"
)
7z a -tzip "Telemachus.Web\public\download\Telemachus-x64.zip" ".\bin\*" -x!Rotativa\* -x!ClientApp\dist\download\* -x!Static\Attachments\*
if exist "Telemachus.Web\public\download\Telemachus-x64-Full.zip" (
    del /q "Telemachus.Web\public\download\Telemachus-x64-Full.zip"
)
7z a -tzip "Telemachus.Web\public\download\Telemachus-x64-Full.zip" ".\bin\*" -x!ClientApp\dist\download\* -x!Static\Attachments\*
rmdir /s /q bin
:error
pause
exit /b %errorlevel%
