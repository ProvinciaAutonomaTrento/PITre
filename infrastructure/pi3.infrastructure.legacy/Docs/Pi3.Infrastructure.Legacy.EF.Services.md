# Package Pi3.Infrastructure.Legacy.EF.Services

## Installa il package

Installa il package tramite NuGet Package Manager:

```
Install-Package Pi3.Infrastructure.Legacy.EF.Services
```

Oppure tramite l'interfaccia della riga di comando di .NET Core:

```
dotnet add package Pi3.Infrastructure.Legacy.EF.Services
```

## Registra il package Pi3.Infrastructure.Legacy.EF.Services con IServiceCollection
Il package Pi3.Infrastructure.Legacy.EF.Services supporta direttamente 
Microsoft.Extensions.DependencyInjection.Abstractions, pertanto è sufficiente utilizzare la sintassi seguente per registrare tutti i tipi e i servizi supportati.

```C#
services.AddInfrastructureLegacyEFServices();
```
Questo registra:
- `IWebMethodLoggerService` come scoped
- `IConfigurationService` come scoped

## Linguaggio, piattaforma e tool di sviluppo
C# 10, .NET 6, Visual Studio 2022.

## Elenco dei package Pi3 inclusi
- [Pi3.Infrastructure.Legacy.EF.Entities](https://gitlab.tndigit.it/tndigit/pitre/pi3.infrastructure.legacy/-/blob/main/Docs/Pi3.Infrastructure.Legacy.EF.Entities.md)
