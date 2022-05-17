using Microsoft.EntityFrameworkCore;
using PaymentService.DataAccess;

var builder = WebApplication.CreateBuilder(args);

// add DBContext
var mariaDbConnectionString = builder.Configuration.GetConnectionString("MariaDbConnectionString");
builder.Services.AddDbContext<PaymentServiceDbContext>(options =>
    options.UseMySql(mariaDbConnectionString, ServerVersion.AutoDetect(mariaDbConnectionString)));


var app = builder.Build();

app.MapGet("/", () => "Hello World from paymentservice!");

app.Run();