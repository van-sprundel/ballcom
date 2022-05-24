using System.Text.Json;
using BallCore.Events;
using BallCore.RabbitMq;
using CustomerManagement;
using CustomerManagement.DataAccess;
using CustomerManagement.Models;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// add DBContext
var mariaDbConnectionString = builder.Configuration.GetConnectionString("MariaDbConnectionString");
builder.Services.AddDbContext<CustomerManagementDbContext>(options =>
    options.UseMySql(mariaDbConnectionString, ServerVersion.AutoDetect(mariaDbConnectionString)));

//Inject receiver
builder.Services.AddHostedService<CustomerMessageReceiver>();

//Inject sender
builder.Services.AddSingleton<IMessageSender, MessageSender>();

// Add framework services
builder.Services
    .AddMvc(options => options.EnableEndpointRouting = false);

// setup MVC
builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseMvc();
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();

app.MapGet("/", () => "Hello World suppliermanagement!");


Console.WriteLine("Starting application");
app.Run();