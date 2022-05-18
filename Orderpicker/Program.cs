using Microsoft.EntityFrameworkCore;
using Orderpicker.DataAccess;

var builder = WebApplication.CreateBuilder(args);

// add DBContext
var mariaDbConnectionString = builder.Configuration.GetConnectionString("MariaDbConnectionString");
builder.Services.AddDbContext<OrderpickerDbContext>(options =>
    options.UseMySql(mariaDbConnectionString, ServerVersion.AutoDetect(mariaDbConnectionString)));


var app = builder.Build();

app.MapGet("/", () => "Hello World from orderpicker!");

app.Run();