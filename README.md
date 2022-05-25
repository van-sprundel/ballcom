# Ball.com

## Instructies voor Developers

### Migrations

Omdat projecten nu afhankelijk zijn van rabbitmq, zullen database updates falen.\
Je zal met `docker-compose up rabbitmq` in de achtergrond migrations moeten doen.\
Ook is de HostName voor nu `localhost`, omdat deze service nu nergens online draait.
### Broker

Om services aan de broker toe te voegen moet je deze in `Program.cs` definiëren.\
De volledige code ziet er zo uit:

```csharp
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
    { "[service]_exchange", new []{ "queue_name" } },
};

builder.Services.AddHostedService(_ => new ExchangeDeclarator(connection, exchanges));

//Inject receivers
builder.Services.AddHostedService<[ExchangeName]MessageReceiver>();
//Inject sender
builder.Services.AddTransient<IMessageSender, MessageSender>();
```

Bij dit deel ga je dus zelf de queue namen en exchange namen bedenken.\
Elke exchange moet weten welke queues hij berichten naar moet versturen.\
In `CustomerManagement` staat al een voorbeeld

```csharp
// create exchange factory
// each exchange needs to know which queues it's going to send data to
var exchanges = new Dictionary<string, IEnumerable<string>>
{
    { "[service]_exchange", new []{ "queue_name" } },
};
```

De receiver zal je zelf ook aan moeten maken.\
Het belangrijkste hier is dat een receiver public class `MessageReceiver` implementeert.\
Je moet namelijk zelf de inkomende data handelen, hier zijn ook al voorbeelden van.

```csharp
//Inject receivers
builder.Services.AddHostedService<[ExchangeName]MessageReceiver>();
```

## Running

### Configure Powershell.

Om scripts uit te voeren in powershell moet het volgende commando in een powershel administrator prompt gerunned worden.

```
Set-ExecutionPolicy Unrestricted
```

Druk vervolgens op `A` om alles te accepteren.

### Bouwen van dotnet en containers

Om alle containers te maken en bij te werken, voer het build-all script uit \
(.ps1 voor windows, .sh voor linux)

### Gebruik van de containers

Om alle containers te starten, voer `docker-compose up` uit\
Om alle containers te stoppen, voer `docker-compose kill` uit (vaak niet nodig)

### RabbitMQ

Om bij de rabbitMq client te komen, ga naar http://localhost:15672/ \
Log in met user:pass `Rathalos`:`1234`

### phpMyAdmin

Om bij phpMyAdmin te komen, ga naar http://localhost:8080/ \
Log in met user:pass `root`:`secret`

### Update databases

- Zorg ervoor dat de MariaDb draaiend is in de container.
- Om te controleren of je entity framework heb, voer de volgende command uit in een powershell window.

```
dotnet ef
```

- als je geen eenhoorn ziet, voer de volgende command uit

```
 dotnet tool install --global dotnet-ef
```

- Als je zelf een migration wil toevoegen, voer de volgende command in een powershell window in de folder van het
  project

```
dotnet ef migrations add naam_migration
```

- Wil je een database updaten met de huidige migrations voer de volgende command uit in de folder van het project.

```
dotnet ef database update
```

- Doe hetzelfde voor:

Wil je alle databases tegelijkertijd updaten, run het script update-databases-all.ps1

### Troubleshoot

```
Error response from daemon: Ports are not available: exposing port TCP 0.0.0.0:3306 -> 0.0.0.0:0: listen tcp 0.0.0.0:3306: bind: Only one usage of each socket address (protocol/network address/port) is normally permitted 
```

Dit gebeurt wanneer er nog een process op dezelfde poort luistert,\
en kan je zien met `netstat -ano | findstr "0.0.0.0:3306"`\
In mijn geval was dit mysqld.exe, dus dit proces moet gekillt worden




