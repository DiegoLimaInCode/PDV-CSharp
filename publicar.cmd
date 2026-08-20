@echo off
setlocal
cd /d "%~dp0"

echo Publicando o PDV para a pasta distri...
dotnet publish PDVCSharp.WPF\PDVCSharp.WPF.csproj -c Release -r win-x64 --self-contained true -p:PublishReadyToRun=true -o distri
if errorlevel 1 (
  echo Falhou o publish. Confira se o .NET 8 SDK esta instalado.
  exit /b 1
)

copy /Y scripts\Instalar.cmd distri\Instalar.cmd >nul
copy /Y scripts\LEIA-ME.txt distri\LEIA-ME.txt >nul

echo.
echo Pronto. O aplicativo esta em C:\PDV-CSharp\distri
echo Clique duas vezes em PDVCSharp.WPF.exe para abrir, ou rode Instalar.cmd
