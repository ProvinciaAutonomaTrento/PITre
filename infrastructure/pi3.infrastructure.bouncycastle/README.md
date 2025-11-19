 # Pi3.Infrastructure.BouncyCastle

Il package implementa i dettagli infrastrutturali per l'utilizzo di [BouncyCastle](https://www.bouncycastle.org/), una libreria gratuita che fornisce un'insieme di api utili per la crittografia.

## Installa il package

Installa il package tramite NuGet Package Manager:

```
Install-Package Pi3.Infrastructure.BouncyCastle
```

Oppure tramite l'interfaccia della riga di comando di .NET Core:

```
dotnet add package Pi3.Infrastructure.BouncyCastle
```

## Registra il package Pi3.Infrastructure.BouncyCastle con IServiceCollection
Il package Pi3.Infrastructure.BouncyCastle supporta direttamente 
Microsoft.Extensions.DependencyInjection.Abstractions, pertanto è sufficiente utilizzare la sintassi seguente per registrare tutti i tipi e i servizi supportati.

```C#
// Registra Pi3.Infrastructure.BouncyCastle 
services.AddInfrastructureBouncyCastle();
```
Questo registra:
- `IEventPublisher` come scoped
- `IFileConverterFactory` come scoped
- `IFileTextExtractorFactory` come scoped
- `IFileTextExtractorService` come scoped
- `ICAdESService` come scoped

## Package Pi3 inclusi

- [Pi3.Core](https://gitlab.tndigit.it/tndigit/pitre/pi3.core/-/blob/main/Docs/Core.md)

## Estrazione del testo
Il package fornisce funzionalità di estrazione del testo da un file firmato .p7m tramite la classe **BouncyCastleSignedFileTextExtractorService**, che implementa l'interfaccia **IFileTextExtractorService**.

### Le interfaccie IFileTextExtractorService e IFileTextExtractorFactory

L'interfaccia **IFileTextExtractorService** permette di estrarre i contenuti testuali da un file.

Il package Pi3.Infrastructure.BouncyCastle fornisce un'implementazione dell'interfaccia per estrarre i contenuti testuali da file firmati digitalmente in formato .p7m.

Gli implementatori definiscono i formati file di input ammessi per l'estrazione del testo mediante il metodo GetSupportedFileFormats.
Il metodo ExtractText permette di estrarre il contenuto testuale presente file nel formato richiesto.

Nel sistema possono essere registrate più implementazioni dell'interfaccia IFileTextExtractorService. Ad esempio, può essere implementato in un package infrastrutturale un estrattore di testo dai formati immagine mentre un altro  package il testo dai file Office o Pdf. Ciascun package potrebbe utilizzare differenti librerie esterne per estrarre il testo.

L'interfaccia **IFileTextExtractorFactory** facilita l'uso degli estrattori poiché restituisce ai client la reale istanza dell'estrattore registrato da utilizzare per ottenere il tsto.

### Utilizzo del servizio di estrazione del testo
Il package implementa il servizio per estrarre il testo da file firmati digitalmente in formato .p7m. Internamente, una volta ottenuto il contenuto del file originale, utilizza IFileTextExtractorFactory per richiedere l'estrazione del testo alle implementazioni di  IFileTextExtractorService registrate.

Per estrarre il testo da un file formato P7M:
```C#
// Ottiene un riferimento alla classe factory
var factory = _serviceProvider.GetRequiredService<IFileTextExtractorFactory>();

string fileName = "NomeFile.pdf.p7m";
byte[] fileContent = new Byte[] { ... }; // Contenuto del file da cui estrarre il contenuto testuale

var creation = await factory.TryCreate(fileName);

if (creation.Success)
{
    var extracted = await creation.Service.ExtractText(fileName, fileContent);

    Console.WriteLine(extracted.ToString());
}
else 
{
    Assert.Fail("Formato file non supportato");
}
```

## Servizi CAdES
Il package fornisce servizi di utilità sui file firmati CAdES tramite la classe **BouncyCastleCAdESService**, che implementa l'interfaccia **ICAdESService**.

### Utilizzo dei servizi di utilità sui file firmati .p7m

Per ottenere il contenuto del file originale da un file in formato P7M, è necessario richiamare il metodo **LoadOriginalFile**:

```C#
// Ottiene un riferimento alla classe factory
var service = _serviceProvider.GetRequiredService<ICAdESService>();

byte[] fileContent = new Byte[] { ... }; // Contenuto del file da cui estrarre il file originale

using var signedFile = new MemoryStream(fileContent);
using var originalFile = new MemoryStream();

await service.LoadOriginalFile(signedFile, originalFile);

// Il contenuto del file originale è in originalFile

Assert.Pass();
```


