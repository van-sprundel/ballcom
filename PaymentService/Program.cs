using BallCore.RabbitMq;
using Microsoft.EntityFrameworkCore;
using PaymentService;
using PaymentService.DataAccess;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

// add DBContext
var mariaDbConnectionString = builder.Configuration.GetConnectionString("MariaDbConnectionString");
builder.Services.AddDbContext<PaymentServiceDbContext>(options =>
    options.UseMySql(
        mariaDbConnectionString,
        ServerVersion.AutoDetect(mariaDbConnectionString)
    )
);

// Create connection
var isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";

var connection = new ConnectionFactory
{
    HostName = isDevelopment ? "localhost" : "rabbitmq" ,
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
    { "payment_exchange", new []{ "payment", "customer" } },
};

builder.Services.AddHostedService(_ => new ExchangeDeclarator(connection, exchanges));

//Inject sender
builder.Services.AddTransient<IMessageSender, MessageSender>();
//Inject receivers
builder.Services.AddHostedService<PaymentMessageReceiver>();


builder.Services
    .AddMvc(options => options.EnableEndpointRouting = false);

builder.Services.AddControllers();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();

app.MapGet("/", () => "Hello World from paymentservice!");

app.Run();