# Pi3.Infrastructure.DocumentFormat.OpenXml

Il package implementa i dettagli infrastrutturali utilizzando la libreria DocumentFormat.OpenXml per la gestione dei formati file office aperti.

## Installa il package

Installa il package tramite NuGet Package Manager:

```
Install-Package Pi3.Infrastructure.DocumentFormat.OpenXml
```

Oppure tramite l'interfaccia della riga di comando di .NET Core:

```
dotnet add package Pi3.Infrastructure.DocumentFormat.OpenXml
```

## Package Pi3 inclusi

- [Pi3.Core](https://gitlab.tndigit.it/tndigit/pitre/pi3.core/-/blob/main/Docs/Core.md)

### L'interfaccia IReportGeneratorService

L'interfaccia **IReportGeneratorService** definisce funzionalità per generare report con testo e tabelle a partire dalle informazioni fornite in ingresso tramite un modello ad oggetti.

L'interfaccia è implementata tramite la classe **OpenXmlReportGeneratorService** che, a partire dai dati ricevuti, genera un report in formato .docx.

### Generazione del report
Le istruzioni seguenti permettono di generare un nuovo report.

```C#
// Ottiene un riferimento al servizio
var reportGeneratorService = _serviceProvider.GetRequiredService<IReportGeneratorService>();

// Definisce un nuovo modello per il report, impostando dimensione ed orientamento delle pagine.
var rm = new ReportModel()
{
    Size = PageSizes.A4,
    Orientation = PageOrientations.Landscape
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

## L'interfaccia ISpreadsheetService

L'interfaccia **ISpreadsheetService** definisce funzionalità per gestire lettura e scrittura report su fogli di calcolo.

L'interfaccia è implementata tramite la classe **OpenXmlShreadsheetService** che, a partire dai dati ricevuti, legge e scrive fogli di calcolo in formato .xlsx.

### Registra il package Pi3.Infrastructure.DocumentFormat.OpenXml con IServiceCollection
Il package Pi3.Infrastructure.DocumentFormat.OpenXml supporta direttamente 
Microsoft.Extensions.DependencyInjection.Abstractions, pertanto è sufficiente utilizzare la sintassi seguente per registrare tutti i tipi e i servizi supportati.

```C#
// Registra Pi3.Infrastructure.DocumentFormat.OpenXml e il servizio di gestione dei file in formato .xlsx.
services.AddInfrastructureOpenXmlSpreadsheetService();
```
Questo registra:
- `IEventPublisher` come scoped
- `IFileConverterFactory` come scoped
- `IFileTextExtractorFactory` come scoped
- `IFileConverterService` come scoped
- `ISpreadsheetService` come scoped

### Lettura dati da un file .xlsx
Le istruzioni seguenti permettono di leggere dati da un file excel in formato .xlsx.

```C#
// Ottiene un riferimento al servizio
var spreadsheetService = _serviceProvider.GetRequiredService<ISpreadsheetService>();

var fileContent = new Byte[] { ... }; // Contenuto del file .xlsx

using var stream = new MemoryStream(fileContent);

// Lettura del contenuto del file nel modello ad oggetti
var model = await service.Read(stream);

Assert.IsNotNull(model);
```

### Scrittura dati in un file .xlsx
Le istruzioni seguenti permettono di scrivere dati in un file excel in formato .xlsx.

```C#
// Ottiene un riferimento al servizio
var spreadsheetService = _serviceProvider.GetRequiredService<ISpreadsheetService>();

// Creazione del modello
var model = new SpreadsheetModel();

// Creazione di un nuovo sheet
var sheetModel1 = new SheetModel();
sheetModel1.Name = "Sheet 1";

// Aggiunta di celle allo sheet, indicando contenuto testuale e formattazione
sheetModel1.AddCell(new CellModel()
{
    Row = 0,
    Column = 0,
    ValueAsString = "Testo di prova",
    CellStyle = new CellStyleModel()
    {
        FontColor = Color.Red,
        FontIsBold = true,
        ForegroundColor = Color.Yellow,
        FontName = "Arial",
        FontSize = 20,
        Width = 50,
        HasBorder = true
    }
});

sheetModel1.AddCell(new CellModel()
{
    Row = 0,
    Column = 1,
    ValueAsString = "Testo di prova",
    CellStyle = new CellStyleModel()
    {
        FontColor = Color.White,
        FontIsBold = true,
        ForegroundColor = Color.Black
    }
});

// Aggiunta dello sheet al modello
model.AddSheet(sheetModel1);

using var stream = new MemoryStream();

// Lettura del contenuto del file nel modello ad oggetti
await service.Write(model, stream);

Assert.Pass();
```