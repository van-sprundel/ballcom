echo "Building CustomerManagement"
cd CustomerManagement
.\build.ps1

echo "Building InventoryManagement"
cd ..\InventoryManagement
.\build.ps1

echo "Building NotificationService"
cd ..\NotificationService
.\build.ps1

echo "Building OrderManagement"
cd ..\OrderManagement
.\build.ps1

echo "Building OrderPicker"
cd ..\Orderpicker
.\build.ps1

echo "Building PaymentService"
cd ..\PaymentService
.\build.ps1

echo "Building ServiceDesk"
cd ..\ServiceDesk
.\build.ps1

echo "Building SupplierManagement"
cd ..\SupplierManagement
.\build.ps1

echo "Building TransportManagement"
cd ..\TransportManagement
.\build.ps1

cd ..
echo "Done!"
echo "Building docker containers with docker-compose"
docker-compose build