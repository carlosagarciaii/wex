#!/bin/bash
set -e

mysqld_safe --user=root &

sleep 10

mysql -u root -p"$MYSQL_ROOT_PASSWORD" < /docker-entrypoint-initdb.d/buildDatabase.sql

exec docker-entrypoint.sh "$@"

