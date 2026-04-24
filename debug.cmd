@echo off
TASKKILL /IM Telemachus.exe /F 2>nul
dotnet publish Telemachus.Api\Telemachus -c Release -r win-x64 -p:PublishSingleFile=true --self-contained false --framework netcoreapp3.1 --force -o debug
pause
