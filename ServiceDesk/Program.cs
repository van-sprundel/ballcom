using Microsoft.EntityFrameworkCore;
using ServiceDesk.DataAccess;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext
var mariaDbConnectionString = builder.Configuration.GetConnectionString("MariaDbConnectionString");
builder.Services.AddDbContext<ServiceDeskDbContext>(options =>
    options.UseMySql(mariaDbConnectionString, ServerVersion.AutoDetect(mariaDbConnectionString)));

var app = builder.Build();

app.MapGet("/", () => "Hello World from servicedesk!");

app.Run();