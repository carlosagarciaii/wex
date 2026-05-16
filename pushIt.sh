#!/bin/bash

createHeader(){
	echo "------------------------------";
	echo "-- $1"
	echo "------------------------------";
}

workingDir="${PWD}";

cd ./source/TransactAPI;

createHeader "Running .Net Format";
dotnet format TransactAPI;

cd "${workingDir}";

createHeader "Standardizing Carriage Returns";
git status | grep -E '^(\s+.+(file|fied):\s)(.+)$' | sed -E 's|.+:\s+||g' | while read file; 
	do 
		dos2unix "$file" --keepdate --safe --oldfile; 
	done;

createHeader "Staging";
git add . ;

createHeader "Commit";
git commit -m "$1";
createHeader "Pushing";
git push;

