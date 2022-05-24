echo "Killing containers..."
docker-compose down

echo "Creating databases"

docker volume rm rabbitmqdata
docker volume rm mariadbdata

docker volume create mariadbdata
docker volume create rabbitmqdata

echo "Done!"