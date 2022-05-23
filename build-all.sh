#!/bin/bash
echo "Starting..."

source ./build.sh CustomerManagement &&
source ./build.sh InventoryManagement &&
source ./build.sh NotificationService &&
source ./build.sh OrderManagement &&
source ./build.sh OrderPicker && 
source ./build.sh PaymentService &&
source ./build.sh ServiceDesk &&
source ./build.sh SupplierManagement &&
source ./build.sh TransportManagement

echo "Done!"
echo "Building docker containers with docker-compose"

docker volume rm rabbitmqdata
docker volume rm mariadbdata

docker volume create --name=mariadbdata
docker volume create --name=rabbitmqdata

echo "Building docker-compose"

docker-compose build