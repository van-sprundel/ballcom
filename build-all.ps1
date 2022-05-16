echo "Building CustomerManagement"
cd CustomerManagement
.\build.sh

echo "Building InventoryManagement"
cd ..\InventoryManagement
.\build.sh

echo "Building NotificationService"
cd ..\NotificationService
.\build.sh

echo "Building OrderManagement"
cd ..\OrderManagement
.\build.sh

echo "Building OrderPicker"
cd ..\Orderpicker
.\build.sh

echo "Building PaymentService"
cd ..\PaymentService
.\build.sh

echo "Building ServiceDesk"
cd ..\ServiceDesk
.\build.sh

echo "Building SupplierManagement"
cd ..\SupplierManagement
.\build.sh

echo "Building TransportManagement"
cd ..\TransportManagement
.\build.sh

cd ..
echo "Done!"
echo "Building docker containers with docker-compose"
docker-compose build