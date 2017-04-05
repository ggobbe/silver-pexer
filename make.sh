echo [Restore Packages]
dotnet restore ./src/SilverPexer/
echo
echo [Build]
dotnet build ./src/SilverPexer/ --configuration Release --framework netcoreapp1.1 --output ../../bin