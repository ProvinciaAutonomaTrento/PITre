# Pi3.Infrastructure.IText

Implementa i dettagli infrastrutturali utilizzando le librerie IText per la gestione dei file in formato pdf.

Sono implementate le seguenti librerie / package:

Libreria / Package | Scopo |
--- | --- |
Pi3.Infrastructure.IText.ReportGenerator | Package per la creazione di report in formato pdf/a. |
Pi3.Infrastructure.IText.Decorator | Package per l'apposizione di layer testuali ad un file in formato pdf. |

## Installa il package

Installa il package tramite NuGet Package Manager:

```
Install-Package Pi3.Infrastructure.IText.ReportGenerator
Install-Package Pi3.Infrastructure.IText.Decorator
```

Oppure tramite l'interfaccia della riga di comando di .NET Core:

```
dotnet add package Pi3.Infrastructure.IText.ReportGenerator
dotnet add package Pi3.Infrastructure.IText.Decorator
```

## Package Pi3 inclusi

- [Pi3.Core](https://gitlab.tndigit.it/tndigit/pitre/pi3.core/-/blob/main/Docs/Core.md)

### L'interfaccia IReportGeneratorService

L'interfaccia **IReportGeneratorService** definisce funzionalità per generare report con testo e tabelle a partire dalle informazioni fornite in ingresso tramite un modello ad oggetti.

L'interfaccia è implementata tramite la classe **ITextReportGeneratorService** che, a partire dai dati ricevuti, genera un report in formato .pdf.

### Generazione del report
Le istruzioni seguenti permettono di generare un nuovo report.

```C#
// Ottiene un riferimento al servizio
var reportGeneratorService = _serviceProvider.GetRequiredService<IReportGeneratorService>();

// Definisce un nuovo modello per il report, impostando dimensione ed orientamento delle pagine.
var rm = new ReportModel()
{
    Size = PageSizes.A4,
    Orientation = PageOrientations.Landscape,
    OutputType = ReportOutputTypes.AsPdf
};

// Aggiunge al modello una sezione testuale tramite **TextSectionModel**, 
// indicandone il contenuto testuale, il font e la giustificazione del testo.
rm.AddSection(new TextSectionModel()
{
    Style = new TextSectionStyleModel()
    {
        Justification = Justifications.Right
    },
    Content = new TextContentModel()
    {
        Value = "Titolo di prova",
        Style = new TextStyleModel()
        {
            FontName = "Arial",
            FontSize = 24, 
            FontIsBold = true,
            FontColor = System.Drawing.Color.Magenta
        }
    }   
});

// Aggiunge una sezione vuota al modello
rm.AddSection(new EmptySectionModel());

// Aggiunge al modello una sezione tabellare tramite **GridSectionModel**, 
// indicando la larghezza dei contenuti in percentuale rispetto alla larghezza della pagina.
var tsm = new GridSectionModel()
{
    Style = new GridSectionStyleModel()
    {
        WithPercentage = 70
    }
};

// Aggiunge una riga con le rispettive celle alla sezione tabellare (nell'esempio, la riga rappresenta l'header)
var row1 = new GridRowModel();
row1.AddCell(new GridCellModel() { Style = new GridCellStyleModel() { WithPercentage = 5, ForegroundColor = System.Drawing.Color.Gray }, Content = new TextContentModel() { Value = "Registro", Style = new TextStyleModel() { FontIsBold = true } } });
row1.AddCell(new GridCellModel() { Style = new GridCellStyleModel() { WithPercentage = 10, ForegroundColor = System.Drawing.Color.Gray }, Content = new TextContentModel() { Value = "Prot. / Id Doc.", Style = new TextStyleModel() { FontIsBold = true } } });
row1.AddCell(new GridCellModel() { Style = new GridCellStyleModel() { WithPercentage = 5, ForegroundColor = System.Drawing.Color.Gray }, Content = new TextContentModel() { Value = "Data", Style = new TextStyleModel() { FontIsBold = true } } });
row1.AddCell(new GridCellModel() {Style = new GridCellStyleModel() { WithPercentage = 30, ForegroundColor = System.Drawing.Color.Gray }, Content = new TextContentModel() { Value = "Oggetto" , Style = new TextStyleModel() { FontIsBold = true } } });
row1.AddCell(new GridCellModel() {Style = new GridCellStyleModel() { WithPercentage = 10, ForegroundColor = System.Drawing.Color.Gray }, Content = new TextContentModel() { Value = "Tipo" , Style = new TextStyleModel() { FontIsBold = true } } });
row1.AddCell(new GridCellModel() {Style = new GridCellStyleModel() { WithPercentage = 10, ForegroundColor = System.Drawing.Color.Gray }, Content = new TextContentModel() { Value = "Mitt. / Dest." , Style = new TextStyleModel() { FontIsBold = true } } });
row1.AddCell(new GridCellModel() {Style = new GridCellStyleModel() { WithPercentage = 10, ForegroundColor = System.Drawing.Color.Gray }, Content = new TextContentModel() { Value = "Codice fascicolo"    } });
row1.AddCell(new GridCellModel() {Style = new GridCellStyleModel() { WithPercentage = 10, ForegroundColor = System.Drawing.Color.Gray }, Content = new TextContentModel() { Value = "Annullato"   } });
row1.AddCell(new GridCellModel() {Style = new GridCellStyleModel() { WithPercentage = 10, ForegroundColor = System.Drawing.Color.Gray }, Content = new TextContentModel() { Value = "File" , Style = new TextStyleModel() { FontIsBold = true } } });
tsm.AddRow(row1);

// Aggiunge righe e celle con i dati alla sezione tabellare
var row2 = new GridRowModel();
row2.AddCell(new GridCellModel() { Content = new TextContentModel() { Value = "PAT" } });
row2.AddCell(new GridCellModel() { Content = new TextContentModel() { Value = "133014903" } });
row2.AddCell(new GridCellModel() { Content = new TextContentModel() { Value = "13/02/2024" } });
row2.AddCell(new GridCellModel() { Style = new GridCellStyleModel() { Justification = Justifications.Center, VerticalAlignment = VerticalAlignments.Center }, Content = new TextContentModel() { Value = "Test per modifica versione", Style = new TextStyleModel() { FontName = "Colibri", FontIsBold = true, FontColor = Color.Green, HighlightColor = Color.Yellow } } });
row2.AddCell(new GridCellModel() { Content = new TextContentModel() { Value = "NP" } });
row2.AddCell(new GridCellModel() { Content = new TextContentModel() { Value = "" } });
row2.AddCell(new GridCellModel() { Content = new TextContentModel() { Value = "6.4-2024-50" } });
row2.AddCell(new GridCellModel() { Content = new TextContentModel() { Value = "" } });
row2.AddCell(new GridCellModel() { Content = new TextContentModel() { Value = "pdf" } });
tsm.AddRow(row2);

// Aggiunge la sezione tabellare al modello dati
rm.AddSection(tsm);

// Genera il report scaricando il contenuto nello stream
using var stream = new MemoryStream();
await reportGenerator.Generate(rm, stream);
```

### Registra il package Pi3.Infrastructure.IText.ReportGenerator con IServiceCollection
Il package Pi3.Infrastructure.IText supporta direttamente 
Microsoft.Extensions.DependencyInjection.Abstractions, pertanto è sufficiente utilizzare la sintassi seguente per registrare tutti i tipi e i servizi supportati.

```C#
services.AddInfrastructureITextReportGenerator()
```

Questo registra:
- `IReportGeneratorService` come scoped

## L'interfaccia IFileDecoratorService

L'interfaccia **IFileDecoratorService** definisce funzionalità per applicare ad un file Pdf uno o più layer di contenuti testuali.

L'interfaccia è implementata tramite la classe **ITextFileDecoratorService** che, a partire dai dati ricevuti, applica layer testuali ad un file pdf.

### Decorazione di un file
Le istruzioni seguenti permettono decorare un file con layer testuali.

```C#
 var decorator = this._serviceProvider.GetService<IFileDecoratorService>();

 var decoration = await decorator.Decorate(
         ".pdf",
         new byte[] { ... }, // Contenuto del file
         new FileDecoratorInstructions(
                 new TextLayer()
                 {
                     Text = "Layer 1",
                     FontName = "COURIER",
                     FontSize = 12f,
                     FontStyle = TextLayerFontStylesEnum.Bold,
                     Position = LayerDefaultPositionsEnum.MiddleLeft,
                     Rotation = LayerRotationsEnum.Degrees90,
                     FontForeColor = new TextLayerFontRgbColor()
                     {
                         R = 51,
                         G = 51,
                         B = 153
                     }
                 },
                 new TextLayer()
                 {
                     Text = "Layer 2",
                     FontName = "Times",
                     FontSize = 21f,
                     FontStyle = TextLayerFontStylesEnum.Italic,
                     Position = LayerDefaultPositionsEnum.MiddleRight,
                     Rotation = LayerRotationsEnum.Degrees90,
                     FontForeColor = new TextLayerFontRgbColor()
                     {
                         R = 255,
                         G = 0,
                         B = 0
                     },
                     PageNumbersToApplyLayer = new int[5] { 1, 3, 5, 7, 9 }
                 }
             ),
      FileDecoratorOutputFormatsEnum.ToPdf);
```

### Registra il package Pi3.Infrastructure.IText con IServiceCollection
Il package Pi3.Infrastructure.IText supporta direttamente 
Microsoft.Extensions.DependencyInjection.Abstractions, pertanto è sufficiente utilizzare la sintassi seguente per registrare tutti i tipi e i servizi supportati.

```C#
services.AddInfrastructureITextFileDecorator()
```

Questo registra:
- `IFileDecoratorService` come scoped