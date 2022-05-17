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
Om alle containers te stoppen, voer `docker-compose kill` uit

# RabbitMQ

Om bij de rabbitMq client te komen, ga naar http://localhost:15672/ \
Log in met user:pass `Rathalos`:`1234`

# phpMyAdmin

Om bij phpMyAdmin te komen, ga naar http://localhost:8080/ \
Log in met user:pass `root`:`secret`