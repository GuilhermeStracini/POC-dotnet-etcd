#!/bin/bash
read -p 'POC name (file name): ' POCName
read -p 'POC name (readable version): ' POCNameReadable

MainProjectFile="Src/POCTemplate/POCTemplate.csproj"
UnitTestProjectFile="Tests/POCTemplate.Tests/POCTemplate.Tests.csproj"
MainDir="Src/POCTemplate"
UnitTestDir="Tests/POCTemplate.Tests"

rm README.md
mv "README.template.md" "README.md"

sed -i "s/POC Template/$POCNameReadable/g" README.md
sed -i "s/POC .NET Template/$POCNameReadable/g" .wakatime-project
sed -i "s/POCTemplate/$POCNameReadable/g" _config.yml

for file in $(find . -type f -name '*.cs*'); do
 sed -i "s/POCTemplate/$POCName/g" "$file"
done

for file in $(find . -type f -name '*.csproj*'); do
 sed -i "s/POCTemplate/$POCName/g" "$file"
done

sed -i "s/POCTemplate/$POCName/" "POCTemplate.sln"
mv "POCTemplate.sln" "$POCName.sln"
mv "$MainProjectFile" "Src/POCTemplate/$POCName.csproj"
mv "$UnitTestProjectFile" "Tests/POCTemplate.Tests/$POCName.Tests.csproj"
mv "$MainDir" "Src/$POCName"
mv "$UnitTestDir" "Tests/$POCName.Tests"

rm initial-setup.bat
rm initial-setup.ps1
rm initial-setup.sh
