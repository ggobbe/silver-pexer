@echo off
echo [Restore Packages]
dotnet restore .\src\SilverPexer\
echo.
echo [Build]
dotnet build .\src\SilverPexer\ --output .\bin --configuration Release --framework net452
echo.
pause