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

docker network rm transact-net
docker network create transact-net

workingDir="${PWD}";

createHeader "Building Database Container"
cd "${workingDir}/database";

docker build -f "mdb.Dockerfile" -t dbsource . || exit 1
docker run -d --name mariadb-container --network transact-net -p 3306:3306 dbsource

createHeader "Building API Container" 
cd "${workingDir}"
docker build -f "./source/TransactAPI/TransactAPI/Dockerfile" -t apisource "./source/TransactAPI/TransactAPI/"
# docker run -d --name transactapi-container -p 5159:8080 -p 7148:8081 apisource
docker run -d --name transactapi-container --network transact-net -p 8080:8080 -p 8081:8081 apisource

createHeader "Sending Validation Start for DB Service" 
sleep 20
docker start mariadb-container



