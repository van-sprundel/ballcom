#!/bin/bash
echo "Updating databases"

dotnet ef database update --project .\CustomerManagement\
dotnet ef database update --project .\ServiceDesk\
dotnet ef database update --project .\SupplierManagement\
dotnet ef database update --project .\InventoryManagement\
dotnet ef database update --project .\Orderpicker\
dotnet ef database update --project .\OrderManagement\
dotnet ef database update --project .\PaymentService\
dotnet ef database update --project .\TransportManagement\

echo "Done!"