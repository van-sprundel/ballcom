echo "Killing containers..."
docker-compose down

echo "Creating databases"

docker volume rm rabbitmqdata
docker volume rm mariadbdata

docker volume create mariadbdata
docker volume create rabbitmqdata

echo "Creating migrations"
docker-compose up rabbitmq mariadb -d

Start-Sleep -Seconds 5

Remove-Item -path "CustomerManagement/Migrations/*" -r
Remove-Item -path "InventoryManagement/Migrations/*" -r
Remove-Item -path "NotificationService/Migrations/*" -r
Remove-Item -path "OrderManagement/Migrations/*" -r
Remove-Item -path "OrderPicker/Migrations/*" -r
Remove-Item -path "PaymentService/Migrations/*" -r
Remove-Item -path "ServiceDesk/Migrations/*" -r
Remove-Item -path "SupplierManagement/Migrations/*" -r
Remove-Item -path "TransportManagement/Migrations/*" -r
Remove-Item -path "EventService/Migrations/*" -r

dotnet ef migrations add "init" --project .\CustomerManagement\
dotnet ef migrations add "init" --project .\ServiceDesk\
dotnet ef migrations add "init" --project .\NotificationService\ #--verbose
dotnet ef migrations add "init" --project .\SupplierManagement\
dotnet ef migrations add "init" --project .\InventoryManagement\
dotnet ef migrations add "init" --project .\Orderpicker\
dotnet ef migrations add "init" --project .\OrderManagement\ #--verbose
dotnet ef migrations add "init" --project .\PaymentService\
dotnet ef migrations add "init" --project .\TransportManagement\
dotnet ef migrations add "init" --project .\EventService\

#.\update-database-all.ps1
docker-compose down
echo "Done!"