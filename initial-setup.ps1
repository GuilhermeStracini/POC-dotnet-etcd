$POCName = Read-Host -Prompt 'POC name (file name)'
$POCNameReadable = Read-Host -Prompt 'POC name (readable version)'
$MainProjectFile = "Src/POCTemplate/POCTemplate.csproj"
$UnitTestProjectFile = "Tests/POCTemplate.Tests/POCTemplate.Tests.csproj"
$MainDir = "Src/POCTemplate"
$UnitTestDir = "Tests/POCTemplate.Tests"

rm README.md
mv "README.template.md" "README.md"
 
(Get-Content README.md) | ForEach-Object {$_ -replace "POC Template", $POCNameReadable} | Set-Content README.md
(Get-Content .wakatime-project) | ForEach-Object {$_ -replace "POC .NET Template", "$POCNameReadable"} | Set-Content .wakatime-project
(Get-Content _config.yml) | ForEach-Object {$_ -replace "POCTemplate", $POCNameReadable} | Set-Content _config.yml
 
(Get-ChildItem -Recurse -Include *.cs*) | ForEach-Object {
 $Content = Get-Content $_
 $Content = $Content -replace "POCTemplate", $POCName
 Set-Content -Path $_.fullname -Value $Content
}

(Get-ChildItem -Recurse -Include *.csproj*) | ForEach-Object {
 $Content = Get-Content $_
 $Content = $Content -replace "POCTemplate", $POCName
 Set-Content -Path $_.fullname -Value $Content
}

(Get-Content POCTemplate.sln) | ForEach-Object {$_ -replace "POCTemplate", $POCName} | Set-Content POCTemplate.sln
Rename-Item -Path ".\POCTemplate.sln" ".\$POCName.sln"
Rename-Item -Path $MainProjectFile -NewName "$POCName.csproj"
Rename-Item -Path $UnitTestProjectFile -NewName "$POCName.Tests.csproj"
Rename-Item -Path $MainDir $POCName
Rename-Item -Path $UnitTestDir "$POCName.Tests"

Remove-Item initial-setup.bat
Remove-Item initial-setup.ps1
Remove-Item initial-setup.sh
