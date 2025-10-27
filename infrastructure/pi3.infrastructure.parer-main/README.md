# Pi3.Infrastructure.ParER

## Introduzione
Il progetto **Pi3.Infrastructure.ParER** raggruppa i package per consentire l'interazione con il conservatore PaRER.

## Linguaggio, piattaforma e tool di sviluppo
C# 10, .NET 6, Visual Studio 2022.

## Installa il package

Installa il package tramite NuGet Package Manager:

```
Install-Package Pi3.Infrastructure.ParER
```

Oppure tramite l'interfaccia della riga di comando di .NET Core:

```
dotnet add package Pi3.Infrastructure.ParER
```

## Registra il package Pi3.Infrastructure.ParER con IServiceCollection
Il package Pi3.Infrastructure.ParER supporta direttamente 
Microsoft.Extensions.DependencyInjection.Abstractions, pertanto è sufficiente utilizzare la sintassi seguente per registrare tutti i tipi e i servizi supportati.

```C#
// Registra Pi3.Infrastructure.ParER 
.AddInfrastructureParERDigitalPreservation(
    "indicare l'url per ai servizi sacer", 
    opt =>
    {
        opt.UserName = "";
        opt.Password = "";
        opt.Version = "";
        opt.VersionGet = "";
        opt.Ambiente = "";
    })
```
Questo registra:
- `ISIPService` come scoped

### Utilizzo del servizio 

Per inviare un documento in conservazione:
```C#
// Ottiene un riferimento al servizio
var service = this._serviceProvider.GetRequiredService<ISIPService>();

var result = await this._service.Send(id); // Id del documento da inviare in conservazione

Assert.IsTrue(result?.Status == Core.Services.DigitalPreservation.DigitalPreservationStatusEnum.Accepted);
```

Per reperire lo stato di conservazione di un documento:
```C#
// Ottiene un riferimento al servizio
var service = this._serviceProvider.GetRequiredService<ISIPService>();

var content = await this._service.Get(id); // Id del documento inviato in conservazione

// Contenuto restituito in formato xml

```

## Package Pi3 inclusi

- [Pi3.Core](https://gitlab.tndigit.it/tndigit/pitre/pi3.core/-/blob/main/Docs/Core.md)