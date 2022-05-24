using BallCore.RabbitMq;
using Microsoft.EntityFrameworkCore;
using PaymentService;
using PaymentService.DataAccess;
using PaymentService.Models;

var builder = WebApplication.CreateBuilder(args);

// add DBContext
var mariaDbConnectionString = builder.Configuration.GetConnectionString("MariaDbConnectionString");
builder.Services.AddDbContext<PaymentServiceDbContext>(options =>
    options.UseMySql(
        mariaDbConnectionString,
        ServerVersion.AutoDetect(mariaDbConnectionString)
    )
);

//Inject receivers
builder.Services.AddHostedService<PaymentMessageReceiver>();
//Inject sender
builder.Services.AddTransient<IMessageSender>(_ => new MessageSender(new [] {"payment","customer"},"payment_exchange"));

builder.Services
    .AddMvc(options => options.EnableEndpointRouting = false);

builder.Services.AddControllers();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();

app.MapGet("/", () => "Hello World from paymentservice!");

app.Run();