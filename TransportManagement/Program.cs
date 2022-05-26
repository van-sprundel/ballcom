using BallCore.RabbitMq;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using TransportManagement;
using TransportManagement.DataAccess;

var builder = WebApplication.CreateBuilder(args);

// add DBContext
var mariaDbConnectionString = builder.Configuration.GetConnectionString("MariaDbConnectionString");
builder.Services.AddDbContext<TransportManagementDbContext>(options =>
    options.UseMySql(mariaDbConnectionString, ServerVersion.AutoDetect(mariaDbConnectionString)));

var isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
if (!isDevelopment)
{
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
        { "transport_exchange_order", new[] { "order_management" } },
    };

    builder.Services.AddHostedService(_ => new ExchangeDeclarator(connection, exchanges));

//Inject receivers
    builder.Services.AddHostedService<TransportMessageReceiver>();

//Inject sender
    builder.Services.AddTransient<IMessageSender, MessageSender>();
}

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

app.MapGet("/", () => "Hello World from transportmanagement!");

app.Run();