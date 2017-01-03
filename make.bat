@echo off
echo [Restore Packages]
dotnet restore .\src\SilverPexer\
echo.
echo [Build]
rem Building twice as a workaround to https://github.com/dotnet/cli/issues/2871
dotnet build .\src\SilverPexer\ --configuration Release --framework net452 > .\build.tmp
dotnet build .\src\SilverPexer\ --configuration Release --framework net452 --output .\bin
del .\build.tmp