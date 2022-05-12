#!/bin/bash

rm -r out
mkdir out
dotnet publish -c Release -r linux-x64 --self-contained true -o out
