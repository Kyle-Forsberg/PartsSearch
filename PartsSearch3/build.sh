#!/bin/bash
echo "building..."
dotnet publish -c Release -r linux-x64
cp ./bin/Release/net8.0/linux-x64/publish/partssearch ~/
echo "executable written to home directory"
