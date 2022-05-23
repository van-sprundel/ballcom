using BallCore.RabbitMq;
using Microsoft.EntityFrameworkCore;
using PaymentService;
using PaymentService.DataAccess;

var builder = WebApplication.CreateBuilder(args);

// add DBContext
var mariaDbConnectionString = builder.Configuration.GetConnectionString("MariaDbConnectionString");
builder.Services.AddDbContext<PaymentServiceDbContext>(options =>
    options.UseMySql(mariaDbConnectionString, ServerVersion.AutoDetect(mariaDbConnectionString)));

//Inject receivers
builder.Services.AddHostedService<OrderMessageReceiver>();

//Inject sender
builder.Services.AddSingleton<IMessageSender, MessageSender>();

builder.Services
    .AddMvc(options => options.EnableEndpointRouting = false);

builder.Services.AddControllers();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();

app.MapGet("/", () => "Hello World from paymentservice!");

app.Run();