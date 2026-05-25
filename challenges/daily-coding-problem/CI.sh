#!/bin/bash
#
dotnet restore --packages packages
dirs=("./01.Arrays ./02.Strings ./03.LinkedLists")

for i in $dirs; do echo $i; done


for dir in $dirs
do
    echo "########## $dir ##########"
    echo ""
    for entry in "$dir"/*
    do
    dotnet fsi "$entry"
    done
done
