using BallCore.RabbitMq;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using ServiceDesk;
using ServiceDesk.DataAccess;
using SupplierManagement.DataAccess;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext
var mariaDbConnectionString = builder.Configuration.GetConnectionString("MariaDbConnectionString");
builder.Services.AddDbContext<ServiceDeskDbContext>(options =>
    options.UseMySql(mariaDbConnectionString, ServerVersion.AutoDetect(mariaDbConnectionString)));

var isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";

if (!isDevelopment)
{
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
        { "order_exchange", new[] { "order" } }
    };

    builder.Services.AddHostedService(_ => new ExchangeDeclarator(connection, exchanges));

//Inject receivers
    builder.Services.AddHostedService<ServiceDeskMessageReceiver>();

//Inject sender
    builder.Services.AddTransient<IMessageSender, MessageSender>();
}

builder.Services.AddControllers();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();

app.MapGet("/", () => "Hello World from servicedesk!");


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<SupplierManagementDbContext>();
    if (context.Database.GetPendingMigrations().Any()) context.Database.Migrate();
}
app.Run();