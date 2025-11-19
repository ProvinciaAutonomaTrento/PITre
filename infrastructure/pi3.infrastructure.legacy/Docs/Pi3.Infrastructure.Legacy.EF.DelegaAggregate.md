# Package Pi3.Infrastructure.Legacy.EF.DelegaAggregate

## Installa il package

Installa il package tramite NuGet Package Manager:

```
Install-Package Pi3.Infrastructure.Legacy.EF.DelegaAggregate
```

Oppure tramite l'interfaccia della riga di comando di .NET Core:

```
dotnet add package Pi3.Infrastructure.Legacy.EF.DelegaAggregate
```

## Registra il package Pi3.Infrastructure.Legacy.EF.DelegaAggregate con IServiceCollection
Il package Pi3.Infrastructure.Legacy.EF.DelegaAggregate supporta direttamente 
Microsoft.Extensions.DependencyInjection.Abstractions, pertanto è sufficiente utilizzare la sintassi seguente per registrare tutti i tipi e i servizi supportati.

```C#
// Registra il servizio di persistenza dell'aggregato Delega tramite EntityFramework
services.AddInfrastructureLegacyEFDelegaAggregate();
```
Questo registra:
- `IDelegaRepository` come scoped

## Linguaggio, piattaforma e tool di sviluppo
C# 10, .NET 6, Visual Studio 2022.

## Elenco dei package Pi3 inclusi
- [Pi3.Core.DelegaAggregate](https://gitlab.tndigit.it/tndigit/pitre/pi3.core/-/blob/main/Docs/Pi3.Core.DelegaAggregate.md)
