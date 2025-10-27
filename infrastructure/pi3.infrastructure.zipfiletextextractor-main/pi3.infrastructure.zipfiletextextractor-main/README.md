 # Pi3.Infrastructure.ZipFileTextExtractor

Il package implementa il servizio di estrazione del testo nei documenti presenti all'interno di archivi .zip utilizzando le classi del namespace di System.Io.Compression.

## Installa il package

Installa il package tramite NuGet Package Manager:

```
Install-Package Pi3.Infrastructure.ZipFileTextExtractor
```

Oppure tramite l'interfaccia della riga di comando di .NET Core:

```
dotnet add package Pi3.Infrastructure.ZipFileTextExtractor
```

## Registra il package Pi3.Infrastructure.ZipFileTextExtractor con IServiceCollection
Il package Pi3.Infrastructure.ZipFileTextExtractor supporta direttamente 
Microsoft.Extensions.DependencyInjection.Abstractions, pertanto è sufficiente utilizzare la sintassi seguente per registrare tutti i tipi e i servizi supportati.

```C#
// Registra Pi3.Infrastructure.ZipFileTextExtractor 
// Indicare tra le opzioni il percorso della cartella temporanea in cui estrarre i file presenti nell'archivio .Zip
services.AddZipFileTextExtractor(opt => opt.SpoolFolder = "******"); 
```
Questo registra:
- `IEventPublisher` come scoped
- `IFileConverterFactory` come scoped
- `IFileTextExtractorFactory` come scoped
- `IFileTextExtractorService` come scoped

## Package Pi3 inclusi

- [Pi3.Core](https://gitlab.tndigit.it/tndigit/pitre/pi3.core/-/blob/main/Docs/Core.md)

## Le interfaccie IFileTextExtractorService e IFileTextExtractorFactory

L'interfaccia **IFileTextExtractorService** permette di estrarre i contenuti testuali da un file.

Il package Pi3.Infrastructure.ZipFileTextExtractor fornisce un'implementazione dell'interfaccia per estrarre i contenuti testuali nei documenti presenti all'interno di un archivio .zip.

Gli implementatori definiscono i formati file di input ammessi per l'estrazione del testo mediante il metodo GetSupportedFileFormats.
Il metodo ExtractText permette di estrarre il contenuto testuale presente file nel formato richiesto.

Nel sistema possono essere registrate più implementazioni dell'interfaccia IFileTextExtractorService. Ad esempio, può essere implementato in un package infrastrutturale un estrattore di testo dai formati immagine mentre un altro  package il testo dai file Office o Pdf. Ciascun package potrebbe utilizzare differenti librerie esterne per estrarre il testo.

L'interfaccia **IFileTextExtractorFactory** facilita l'uso degli estrattori poiché restituisce ai client la reale istanza dell'estrattore registrato da utilizzare per ottenere il tsto.

## Utilizzo del servizio di estrazione del testo
Il package implementa il servizio per estrarre il testo da file in formato .zip. Internamente, una volta estratti i documenti presenti nell'archivio, utilizza a sua volta IFileTextExtractorFactory per richiedere l'estrazione del testo alle implementazioni di  IFileTextExtractorService registrate.

Per estrarre il testo da un file formato .zip:
```C#
// Ottiene un riferimento alla classe factory
var factory = _serviceProvider.GetRequiredService<IFileTextExtractorFactory>();

string fileName = "NomeFile.zip";
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
