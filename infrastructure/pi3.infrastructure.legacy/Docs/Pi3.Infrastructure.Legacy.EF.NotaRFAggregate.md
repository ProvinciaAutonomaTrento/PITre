# Package Pi3.Infrastructure.Legacy.EF.NotaRFAggregate

## Installa il package

Installa il package tramite NuGet Package Manager:

```
Install-Package Pi3.Infrastructure.Legacy.EF.NotaRFAggregate
```

Oppure tramite l'interfaccia della riga di comando di .NET Core:

```
dotnet add package Pi3.Infrastructure.Legacy.EF.NotaRFAggregate
```

## Registra il package Pi3.Infrastructure.Legacy.EF.NotaRFAggregate con IServiceCollection
Il package Pi3.Infrastructure.Legacy.EF.NotaRFAggregate supporta direttamente 
Microsoft.Extensions.DependencyInjection.Abstractions, pertanto è sufficiente utilizzare la sintassi seguente per registrare tutti i tipi e i servizi supportati.

```C#
// Registra il servizio di persistenza dell'aggregato NotaRFAggregate tramite EntityFramework
services.AddInfrastructureLegacyEFNotaRFAggregate();
```
Questo registra:
- `IKeywordRepository` come scoped

## Linguaggio, piattaforma e tool di sviluppo
C# 10, .NET 6, Visual Studio 2022.

## Elenco dei package Pi3 inclusi
- [Pi3.Core.KeywordAggregate](https://gitlab.tndigit.it/tndigit/pitre/pi3.core/-/blob/main/Docs/Pi3.Core.NotaRFAggregate.md)
