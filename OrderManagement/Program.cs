using Microsoft.EntityFrameworkCore;
using OrderManagement.DataAccess;

var builder = WebApplication.CreateBuilder(args);

// add DBContext
var mariaDbConnectionString = builder.Configuration.GetConnectionString("MariaDbConnectionString");
builder.Services.AddDbContext<OrderManagementDbContext>(options =>
    options.UseMySql(mariaDbConnectionString, ServerVersion.AutoDetect(mariaDbConnectionString)));


var app = builder.Build();

app.MapGet("/", () => "Hello World from ordermanagement!");

app.Run();