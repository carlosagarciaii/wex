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

sleep 5

workingDir="${PWD}";

createHeader "Building Database Container"
cd "${workingDir}/database";

docker build -f "mdb.Dockerfile" -t dbsource . || exit 1
docker run -d --name mariadb-container -p 3306:3306 dbsource

createHeader "Starting .Net Unit Tests" 

cd "${workingDir}/source/TransactAPI/TransactAPITests"

dotnet test TransactAPITests.csproj > ~/Development/testResults.txt


