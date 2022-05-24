#!/bin/bash
echo "Starting..."

source ./build.sh CustomerManagement &&
source ./build.sh InventoryManagement &&
source ./build.sh NotificationService &&
source ./build.sh OrderManagement &&
source ./build.sh Orderpicker && 
source ./build.sh PaymentService &&
source ./build.sh ServiceDesk &&
source ./build.sh SupplierManagement &&
source ./build.sh TransportManagement

echo "Done!"
echo "Building docker containers with docker-compose"


echo "Building docker-compose"

docker-compose build