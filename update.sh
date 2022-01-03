#!/bin/sh
git pull
chmod a+x update.sh
dotnet publish -c Release
cd bin/Release/net6.0
pm2 stop "dotnet fixr"
pm2 delete "dotnet fixr"
pm2 start "dotnet fixr.dll"
pm2 save