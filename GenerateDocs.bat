.\Packages\docfx.console.2.56.7\tools\docfx.exe docfx.json
CHOICE /M "Serve docs at localhost:8080?"
IF ERRORLEVEL 2 EXIT
IF ERRORLEVEL 1 .\Packages\docfx.console.2.56.7\tools\docfx.exe serve ./docs
