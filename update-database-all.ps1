echo "Updating databases"

cd .\CustomerManagement\
dotnet ef database update
cd ..

cd .\ServiceDesk\
dotnet ef database update
cd ..

cd .\SupplierManagement\
dotnet ef database update
cd ..

cd .\InventoryManagement\
dotnet ef database update
cd ..

cd .\Orderpicker\
dotnet ef database update
cd ..

cd .\OrderManagement\
dotnet ef database update
cd ..

cd .\PaymentService\
dotnet ef database update
cd ..

cd .\TransportManagement\
dotnet ef database update
cd ..

echo "Done!"