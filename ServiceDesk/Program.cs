using Microsoft.EntityFrameworkCore;
using ServiceDesk.DataAccess;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext
var mariaDbConnectionString = builder.Configuration.GetConnectionString("MariaDbConnectionString");
builder.Services.AddDbContext<ServiceDeskDbContext>(options =>
    options.UseMySql(mariaDbConnectionString, ServerVersion.AutoDetect(mariaDbConnectionString)));

builder.Services.AddControllers();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();

app.MapGet("/", () => "Hello World from servicedesk!");

app.Run();