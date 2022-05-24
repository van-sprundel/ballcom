echo "Starting..."

#.\build.ps1 CustomerManagement 
#.\build.ps1 InventoryManagement  
#.\build.ps1 NotificationService  
.\build.ps1 OrderManagement 
#.\build.ps1 Orderpicker 
#.\build.ps1 PaymentService 
#.\build.ps1 ServiceDesk 
#.\build.ps1 SupplierManagement
#.\build.ps1 TransportManagement

echo "Done!"
echo "Building docker containers with docker-compose"

docker volume rm rabbitmqdata
docker volume rm mariadbdata

docker volume create mariadbdata
docker volume create rabbitmqdata

docker-compose build