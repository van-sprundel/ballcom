using BallCore.RabbitMq;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OrderManagement;
using OrderManagement.DataAccess;
using RabbitMQ.Client;


var builder = WebApplication.CreateBuilder(args);

// add DBContext
var mariaDbConnectionString = builder.Configuration.GetConnectionString("MariaDbConnectionString");
builder.Services.AddDbContext<OrderManagementDbContext>(options =>
    options.UseMySql(mariaDbConnectionString, ServerVersion.AutoDetect(mariaDbConnectionString)));

// Create connection
var connection = new ConnectionFactory
{
    HostName = "rabbitmq",
    Port = 5672,
    UserName = "Rathalos",
    Password = "1234",
    DispatchConsumersAsync = true
}.CreateConnection();

builder.Services.AddSingleton(connection);

// create exchange factory
// each exchange needs to know which queues it's going to send data to
var exchanges = new Dictionary<string, IEnumerable<string>>
{
    { "order_exchange_order", new []{ "orderpicker_client", "transport_management", "notifications" } },
    { "order_exchange_order_product", new []{ "orderpicker_client" } },
};

builder.Services.AddHostedService(_ => new ExchangeDeclarator(connection, exchanges));

//Inject receivers
builder.Services.AddHostedService<OrderMessageReceiver>();

//Inject sender
builder.Services.AddTransient<IMessageSender, MessageSender>();


// Add framework Services
builder.Services
    .AddMvc(options => options.EnableEndpointRouting = false)
    .AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

// Setup MVC
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

app.MapGet("/", () => "Hello World from ordermanagement!");

app.Run();