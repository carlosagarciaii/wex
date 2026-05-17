FROM mariadb:latest

COPY ./database /docker-entrypoint-initdb.d/


ENV MYSQL_ROOT_PASSWORD=rootpassword \
    MYSQL_DATABASE=database \
    MYSQL_USER=dbuser \
    MYSQL_PASSWORD=dbpassword


