@echo off
echo [Restore Packages]
dotnet restore .\src\SilverPexer\
echo.
echo [Build]
dotnet build .\src\SilverPexer\ --configuration Release --framework net462 --output ..\..\bin