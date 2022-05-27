#!/bin/bash

echo "Killing containers..."
docker-compose down

echo "Creating databases"

docker volume rm rabbitmqdata
docker volume rm mariadbdata

docker volume create mariadbdata
docker volume create rabbitmqdata

echo "Starting mariadb"
docker-compose up rabbitmq mariadb -d

sleep 5

echo "Creating migrations"

rm -r CustomerManagement/Migrations
rm -r InventoryManagement/Migrations
rm -r NotificationService/Migrations
rm -r OrderManagement/Migrations
rm -r OrderPicker/Migrations
rm -r PaymentService/Migrations
rm -r ServiceDesk/Migrations
rm -r SupplierManagement/Migrations
rm -r TransportManagement/Migrations
rm -r EventService/Migrations

declare -a arr=("CustomerManagement" "SupplierManagement" "ServiceDesk" "InventoryManagement" "Orderpicker" "OrderManagement" "PaymentService" "TransportManagement")

for i in "${arr[@]}"
do
   cd "$i"
   dotnet ef migrations add "init"
   dotnet ef database update
   cd ..
done

echo "Done!"
