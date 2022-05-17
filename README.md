# ONE TIME ONLY!

# Configure Powershell.

Om scripts uit te voeren in powershell moet het volgende commando in een powershel administrator prompt gerunned worden.

```
Set-ExecutionPolicy Unrestricted
```

Druk vervolgens op `A` om alles te accepteren.

# Bouwen van dotnet en containers

Om alle containers te maken en bij te werken, voer het build-all script uit \
(.ps1 voor windows, .sh voor linux)

# Gebruik van de containers

Om alle containers te starten, voer `docker-compose up` uit\
Om alle containers te stoppen, voer `docker-compose kill` uit (vaak niet nodig)

# RabbitMQ

Om bij de rabbitMq client te komen, ga naar http://localhost:15672/ \
Log in met user:pass `Rathalos`:`1234`

# phpMyAdmin
Om bij phpMyAdmin te komen, ga naar http://localhost:8080/ \
Log in met user:pass `root`:`secret`

# Migration CustomerManagment
- Zorg ervoor dat de MariaDb draaiend is in de container.
- Open een powershell window
- Navigeer naar de map customer management
- Om te controleren of je entity framework heb, voer de volgende command uit.
```
dotnet ef
```
- als je geen eenhoorn ziet, voer de volgende command uit
```
 dotnet tool install --global dotnet-ef
```
- Als je zelf een migration wil toevoegen, voer uit
```
dotnet migrations add "naam_migration"
```
- Wil je de database updaten met de huidige migrations voer dan uit
```
dotnet ef database update
```

# Troubleshoot
```
Error response from daemon: Ports are not available: exposing port TCP 0.0.0.0:3306 -> 0.0.0.0:0: listen tcp 0.0.0.0:3306: bind: Only one usage of each socket address (protocol/network address/port) is normally permitted 
```
Dit gebeurt wanneer er nog een process op dezelfde poort luistert, en kan je zien met `netstat -ano | findstr "0.0.0.0:3306"`\
In mijn geval was dit mysqld.exe, dus dit proces moet gekillt worden




