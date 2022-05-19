#!/bin/bash

dotnet publish $1 -c Release -r linux-musl-x64 --self-contained true -o "$1"/out