# Package Pi3.Infrastructure.Legacy.DocumentFSRepository

## Installa il package

Installa il package tramite NuGet Package Manager:

```
Install-Package Pi3.Infrastructure.Legacy.DocumentFSRepository
```

Oppure tramite l'interfaccia della riga di comando di .NET Core:

```
dotnet add package Pi3.Infrastructure.Legacy.DocumentFSRepository
```

## Registra il package Pi3.Infrastructure.DocumentFSRepository con IServiceCollection
Il package Pi3.Infrastructure.DocumentFSRepository supporta direttamente 
Microsoft.Extensions.DependencyInjection.Abstractions, pertanto è sufficiente utilizzare la sintassi seguente per registrare tutti i tipi e i servizi supportati.

```C#
// Registra Pi3.Infrastructure.Legacy.DocumentFSRepository con il servizio di gestione del repository documentale su file system
services.AddInfrastructureLegacyDocumentBlobFileSystemRepository();
```
Questo registra:
- `IDocumentBlobRepository` come scoped

##Linguaggio, piattaforma e tool di sviluppo
C# 10, .NET 6, Visual Studio 2022.

## Elenco dei package Pi3 inclusi
- [Pi3.Core.DocumentBlobAggregate](https://gitlab.tndigit.it/tndigit/pitre/pi3.core/-/blob/main/Docs/Pi3.Core.DocumentBlobAggregate.md)

