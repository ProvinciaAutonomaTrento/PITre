# Pi3.Infrastructure.Adobe

Il package implementa i dettagli infrastrutturali per l'utilizzo della piattaforma Adobe.

## Installa il package

Installa il package tramite NuGet Package Manager:

```
Install-Package Pi3.Infrastructure.Adobe
```

Oppure tramite l'interfaccia della riga di comando di .NET Core:

```
dotnet add package Pi3.Infrastructure.Adobe
```

## Registra il package Pi3.Infrastructure.Adobe con IServiceCollection
Il package Pi3.Infrastructure.Adobe supporta direttamente 
Microsoft.Extensions.DependencyInjection.Abstractions, pertanto è sufficiente utilizzare la sintassi seguente per registrare tutti i tipi e i servizi supportati.

```C#
// Registra Pi3.Core e il servizio di conversione tramite Adobe
services.AddInfrastructureAdobeFileConverter(cfg =>
                {
                    cfg.ServiceUrl = ""; // Indica l'url del servizio di conversione Adobe
                    // Parametri richiesti dal servizio di conversione Adobe
                    cfg.ServiceCredentialsUserName = ""; 
                    cfg.ServiceCredentialsPassword = "";
                    cfg.PdfSettings = "PDFA1b 2005 CMYK"; 
                    cfg.FileTypeSettings = "Standard PITRE";
                    cfg.SecuritySettings = "No Security";
                    cfg.RequestTimeoutInSeconds = "300"; // Default, timeout impostato 5 minuti
                });
```
Questo registra:
- `IEventPublisher` come scoped
- `IFileConverterFactory` come scoped
- `IFileTextExtractorFactory` come scoped
- `IFileConverterService` come scoped

## Package Pi3 inclusi

- [Pi3.Core](https://gitlab.tndigit.it/tndigit/pitre/pi3.core/-/blob/main/Docs/Core.md)


## Le interfaccie IFileConverterService e IFileTextConverterFactory

L'interfaccia **IFileConverterService** definisce la modalità di conversione del contenuto di un file in formato PDF oppure in formato Immagine. 

Gli implementatori definiscono i formati file di input ammessi dal convertitore mediante i metodi GetSupportedInputFileFormats e IsSupportedInputFileFormat.
Il metodo Convert permette di effettuare la conversione del contenuto del file nel formato richiesto. Solleva un'eccezione se  il formato di input oppure il formato di output non è supportato.

Nel sistema possono essere registrate più implementazioni dell'interfaccia IFileConverterService. Ad esempio, può essere implementato in un package infrastrutturale un convertitore dai formati immagine mentre in un altro un convertitore dai formati Office. Ciascun package potrebbe utilizzare differenti librerie esterne per la conversione.

L'interfaccia **IFileConverterFactory** facilita l'uso dei convertitori poiché restituisce ai client la reale istanza del convertitore registrato da utilizzare per il formato file da convertire.

### Utilizzo del servizio di conversione
Il package implementa il servizio di conversione in PDF utilizzando internamente i servizi esposti dalla piattaforma Adobe. La conversione al formato di output di tipo immagine non è supportata.

Per convertire un file in formato PDF tramite il convertitore:
```C#

// Ottiene un riferimento alla classe factory
var factory = _serviceProvider.GetRequiredService<IFileConverterFactory>();

// Ottiene dalla factory il servizio di conversione specifico per il formato richiesto 
var creation = await factory.TryCreate("Test_conversione_pdf.docx");

// Se il servizio di conversione per il formato richiesto è stato registrato
if (creation.Success && creation.Service != null)
{
    // Effettua la conversione tramite il servizio di conversione
    FileConvertedContent? converted = await creation.Service.Convert(
                    $"Test_conversione_pdf.docx",
                    FilesResources.Test_conversione_pdf,
                    FileConverterOutputFormatsEnum.ToPdf);

    Assert.IsNotNull(converted);
}
else
{
    Assert.Fail();
}
```

### Elenco dei formati file ammessi in conversione

Descrizione | Formato
--- | --- 
Immagini | ".bmp",".gif",".jpeg",".jpg",".tif",".tiff",".png",".jpf",".jpx",".jp2",".j2k",".j2c",".jpc" 
AutoCAD | ".dwg",".dxf",".dwf"
AdobeFlash | ".swf",".flv"
MSExcel | ".xls",".xlsx"
MSPowerpoint | ".ppt",".pptx"
MSProject | ".mpp"
MSPublisher | ".pub"
MSVisio | ".vsd"
MSWord | ".doc",".docx",".rtf",".txt"
OpenOffice | ".odt",".odp",".ods",".odg",".odf",".sxw",".sxi",".sxc",".sxd"
WordPerfect | ".wpd"
PageMaker | ".pmd",".pm6",".p65",".pm"
FrameMaker | ".fm"
Photoshop | ".psd"
Notepad | ".xml"
Html | ".htm", ".html"

