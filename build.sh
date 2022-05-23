#!/bin/bash
MSG="Building "$1
echo "$MSG"

if [ ! -d "$1"/out ];then 
  mkdir "$1"/out
fi

rm -r "$1"/out
source ./build-dotnet.sh $1 