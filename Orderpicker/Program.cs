using BallCore.RabbitMq;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Orderpicker;
using Orderpicker.DataAccess;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

// add DBContext
var mariaDbConnectionString = builder.Configuration.GetConnectionString("MariaDbConnectionString");
builder.Services.AddDbContext<OrderpickerDbContext>(options =>
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
        { "orderpicker_exchange", new[] { "order_management", "inventory_management", "general" } },
    };

    builder.Services.AddHostedService(_ => new ExchangeDeclarator(connection, exchanges));

//Inject receivers
    builder.Services.AddHostedService<OrderPickerReceiver>();

//Inject sender
    builder.Services.AddTransient<IMessageSender, MessageSender>();
}

// Add framework Services
builder.Services
    .AddMvc(options => options.EnableEndpointRouting = false);

// Setup MVC
builder.Services.AddControllers()
    .AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);


var app = builder.Build();

if (app.Environment.IsDevelopment()) app.UseDeveloperExceptionPage();

app.UseMvc();
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<OrderpickerDbContext>();
    if (context.Database.GetPendingMigrations().Any()) context.Database.Migrate();
}


app.MapGet("/", () => "Hello World from orderpicker!");

app.Run();