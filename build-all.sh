#!/bin/bash

echo "Building CustomerManagement"
cd CustomerManagement
source ./build.sh

echo "Building InventoryManagement"
cd ../InventoryManagement
source ./build.sh

echo "Building NotificationService"
cd ../NotificationService
source ./build.sh

echo "Building OrderManagement"
cd ../OrderManagement
source ./build.sh

echo "Building OrderPicker"
cd ../Orderpicker
source ./build.sh

echo "Building PaymentService"
cd ../PaymentService
source ./build.sh

echo "Building ServiceDesk"
cd ../ServiceDesk
source ./build.sh

echo "Building SupplierManagement"
cd ../SupplierManagement
source ./build.sh

echo "Building TransportManagement"
cd ../TransportManagement
source ./build.sh

cd ..
echo "Done!"
echo "Building docker containers with docker-compose"

docker volume rm rabbitmqdata
docker volume rm mariadbdata

docker volume create --name=mariadbdata
docker volume create --name=rabbitmqdata


docker-compose build