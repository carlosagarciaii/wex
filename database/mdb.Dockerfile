FROM mariadb:latest
ENV MYSQL_ROOT_PASSWORD=rootpassword \
    MYSQL_DATABASE=transact \
    MYSQL_USER=dbuser \
    MYSQL_PASSWORD=dbpassword
COPY buildDatabase.sql  /docker-entrypoint-initdb.d/