echo "Killing containers..."
docker-compose down

echo "Creating databases"

docker volume rm rabbitmqdata
docker volume rm mariadbdata

docker volume create mariadbdata
docker volume create rabbitmqdata

echo "Creating migrations"

Start-Sleep -Seconds 5

Remove-Item -path "CustomerManagement/Migrations/*" -r
Remove-Item -path "InventoyManagement/Migrations/*" -r 
Remove-Item -path "NotifcationService/Migrations/*" -r
Remove-Item -path "OrderManagement/Migrations/*" -r
Remove-Item -path "OrderPicker/Migrations/*" -r
Remove-Item -path "PaymentService/Migrations/*" -r
Remove-Item -path "ServiceDesk/Migrations/*" -r
Remove-Item -path "SupplierManagement/Migrations/*" -r
Remove-Item -path "TransportManagement/Migrations/*" -r

dotnet ef migrations add "init" --project .\CustomerManagement\ &&
dotnet ef migrations add "init" --project .\ServiceDesk\ &&
dotnet ef migrations add "init" --project .\SupplierManagement\ &&
dotnet ef migrations add "init" --project .\InventoryManagement\ &&
dotnet ef migrations add "init" --project .\Orderpicker\ &&
dotnet ef migrations add "init" --project .\OrderManagement\ &&
dotnet ef migrations add "init" --project .\PaymentService\ &&
dotnet ef migrations add "init" --project .\TransportManagement\ 

echo "Done!"