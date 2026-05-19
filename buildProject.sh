#!/bin/bash

createHeader(){
	DIV="--------------------------------";
	echo "";
	echo "";
	echo "$DIV";
	echo "-- $1";
	echo "$DIV";
}

docker rm mariadb-container --force
docker rmi dbsource --force
docker rm transactapi-container --force
docker rmi apisource --force

sleep 5

workingDir="${PWD}";

createHeader "Building Database Container"
cd "${workingDir}/database";

docker build -f "mdb.Dockerfile" -t dbsource . || exit 1
docker run -d --name mariadb-container -p 3306:3306 dbsource

createHeader "Building API Container" 
cd "${workingDir}/source"
docker build -f "./TransactAPI/TransactAPI/Dockerfile" -t apisource . || exit 1
docker run -d --name transactapi-container apisource

sleep 20
docker start mariadb-container



