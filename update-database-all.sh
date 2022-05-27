echo "Updating databases"

#dotnet ef migrations add "init" --project .\CustomerManagement\
#dotnet ef migrations add "init" --project .\ServiceDesk\
#dotnet ef migrations add "init" --project .\SupplierManagement\
#dotnet ef migrations add "init" --project .\InventoryManagement\
#dotnet ef migrations add "init" --project .\Orderpicker\
#dotnet ef migrations add "init" --project .\OrderManagement\
#dotnet ef migrations add "init" --project .\PaymentService\
#dotnet ef migrations add "init" --project .\TransportManagement\

dotnet ef database update --project .\CustomerManagement
dotnet ef database update --project .\ServiceDesk
dotnet ef database update --project .\SupplierManagement
dotnet ef database update --project .\InventoryManagement
dotnet ef database update --project .\Orderpicker
dotnet ef database update --project .\OrderManagement
dotnet ef database update --project .\PaymentService
dotnet ef database update --project .\TransportManagement
dotnet ef database update --project .\EventService\

echo "Done!"