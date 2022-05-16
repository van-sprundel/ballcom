var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World from customermanagement!");

Console.WriteLine("Starting application");
app.Run();