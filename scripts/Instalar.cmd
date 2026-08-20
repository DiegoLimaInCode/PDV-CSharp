@echo off
setlocal
cd /d "%~dp0"

set DEST=%LOCALAPPDATA%\PDV-Supermarket
echo Instalando o PDV em %DEST%
if not exist "%DEST%" mkdir "%DEST%"
xcopy /E /Y /Q "%~dp0*" "%DEST%\" >nul

set SHORTCUT=%USERPROFILE%\Desktop\PDV Supermarket.lnk
powershell -NoProfile -Command ^
  "$ws = New-Object -ComObject WScript.Shell; $s = $ws.CreateShortcut('%SHORTCUT%'); $s.TargetPath = '%DEST%\PDVCSharp.WPF.exe'; $s.WorkingDirectory = '%DEST%'; $s.Description = 'PDV Supermarket'; $s.Save()"

echo.
echo Instalado. Atalho criado na Area de Trabalho.
echo Banco de dados: arquivo pdv.db na pasta do programa. Nao precisa de MySQL nem Docker.
start "" "%DEST%\PDVCSharp.WPF.exe"
