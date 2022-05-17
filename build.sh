#!/bin/bash

MSG="Building "$1
echo "$MSG"

if [ ! -d "$1"/out ];then 
  mkdir "$1"/out
fi

rm -r "$1"/out
dotnet publish -c Release -r linux-musl-x64 --self-contained true -o "$1"/out