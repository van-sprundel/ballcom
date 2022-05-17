using Microsoft.EntityFrameworkCore;
using SupplierManagement.DataAccess;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext
var mariaDbConnectionString = builder.Configuration.GetConnectionString("MariaDbConnectionString");
builder.Services.AddDbContext<SupplierManagementDbContext>(options =>
    options.UseMySql(mariaDbConnectionString, ServerVersion.AutoDetect(mariaDbConnectionString)));


var app = builder.Build();

app.MapGet("/", () => "Hello World suppliermanagement!");

app.Run();