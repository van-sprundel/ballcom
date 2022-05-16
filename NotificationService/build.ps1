rm -r out
mkdir out
dotnet publish -c Release -r linux-musl-x64 --self-contained true -o out