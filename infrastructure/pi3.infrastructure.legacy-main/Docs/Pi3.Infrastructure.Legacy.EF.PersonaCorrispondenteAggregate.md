# Package Pi3.Infrastructure.Legacy.EF.PersonaCorrispondenteAggregate

## Installa il package

Installa il package tramite NuGet Package Manager:

```
Install-Package Pi3.Infrastructure.Legacy.EF.PersonaCorrispondenteAggregate
```

Oppure tramite l'interfaccia della riga di comando di .NET Core:

```
dotnet add package Pi3.Infrastructure.Legacy.EF.PersonaCorrispondenteAggregate
```

## Registra il package Pi3.Infrastructure.Legacy.EF.PersonaCorrispondenteAggregate con IServiceCollection
Il package Pi3.Infrastructure.Legacy.EF.PersonaCorrispondenteAggregate supporta direttamente 
Microsoft.Extensions.DependencyInjection.Abstractions, pertanto è sufficiente utilizzare la sintassi seguente per registrare tutti i tipi e i servizi supportati.

```C#
// Registra il servizio di persistenza dell'aggregato PersonaCorrispondenteAggregate tramite EntityFramework
services.AddInfrastructureLegacyEFPersonaCorrispondenteAggregate();
```
Questo registra:
- `IKeywordRepository` come scoped

## Linguaggio, piattaforma e tool di sviluppo
C# 10, .NET 6, Visual Studio 2022.

## Elenco dei package Pi3 inclusi
- [Pi3.Core.PersonaCorrispondenteAggregate](https://gitlab.tndigit.it/tndigit/pitre/pi3.core/-/blob/main/Docs/Pi3.Core.PersonaCorrispondenteAggregate.md)
