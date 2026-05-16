#!/bin/bash

createHeader(){
	DIV="--------------------------------";
	echo "$DIV";
	echo "-- $1";
	echo "$DIV";
}

workingDir="${PWD}";

# cd "${workingDir}/database";

docker build -f "${workingDir}/database/mdb.Dockerfile" -t dbsource . || exit 1
docker run -d --name mariadb-container -p 3306:3306 dbsource



