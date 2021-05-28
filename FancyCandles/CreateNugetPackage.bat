echo OFF
echo;
echo "Don't forget to update the FancyCandles.nuspec file and to build a Release version."
echo;
echo ON

nuget.exe config -Set repositoryPath="C:\Users\DDD\.nuget\packages"
nuget.exe pack -IncludeReferencedProjects -properties Configuration=Release
pause
start opera https://www.nuget.org/packages/manage/upload