@echo off
echo usage : sonar [port=9000]

set port=%1
IF "%port%"=="" set port=9000
set msbuild="%ProgramFiles(x86)%\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin\MSBuild.exe"

dotnet sonarscanner begin /k:"Nemeio" /n:"Nemeio" /v:"1.0.0" /d:sonar.host.url="http://localhost:%port%/" /d:sonar.login="admin" /d:sonar.password="admin"  /d:sonar.exclusions="*.js,*.html,*.css,Nemeio.Mac/StaticLibrary/WKeyboard.cs,*.php,*.xml,*/*.Test,*/WKeyboard,*/Pipelines,*.cake,*.sh,*.bat" 
%msbuild% Nemeio.sln /t:Rebuild -verbosity:quiet -restore:true /property:Configuration=SonarCloud /property:Platform=x64
dotnet sonarscanner end /d:sonar.login="admin" /d:sonar.password="admin" 

