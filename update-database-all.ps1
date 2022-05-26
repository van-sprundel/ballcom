#!/bin/bash
echo "Updating databases"

#dotnet ef migrations add "init3" --project .\CustomerManagement\
#dotnet ef migrations add "init3" --project .\ServiceDesk\
#dotnet ef migrations add "init3" --project .\SupplierManagement\
#dotnet ef migrations add "init3" --project .\InventoryManagement\
#dotnet ef migrations add "init3" --project .\Orderpicker\
#dotnet ef migrations add "init3" --project .\OrderManagement\
#dotnet ef migrations add "init3" --project .\PaymentService\
#dotnet ef migrations add "init3" --project .\TransportManagement\

dotnet ef database update --project .\CustomerManagement\
dotnet ef database update --project .\ServiceDesk\
dotnet ef database update --project .\SupplierManagement\
dotnet ef database update --project .\InventoryManagement\
dotnet ef database update --project .\Orderpicker\
dotnet ef database update --project .\OrderManagement\
dotnet ef database update --project .\PaymentService\
dotnet ef database update --project .\TransportManagement\

echo "Done!"