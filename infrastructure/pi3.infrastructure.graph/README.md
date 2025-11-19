# Pi3.Infrastructure.Graph

## Introduzione
Il progetto **Pi3.Infrastructure.Graph** raggruppa i package per l'interazione con le funzionalità di Microsoft Graph (https://learn.microsoft.com/en-us/graph/use-the-api). In questa versione, le API di Microsoft Graph vengono utilizzate per inviare messaggi di posta elettronica tramite caselle Outlook, attraverso l'implementazione dell'interfaccia IEmailSenderService, e per eseguire la scansione di una casella di posta Outlook, tramite l'interfaccia IEmailBoxScannerService.

## Installa il package

Installa il package tramite NuGet Package Manager:

```
Install-Package Pi3.Infrastructure.Graph
```

Oppure tramite l'interfaccia della riga di comando di .NET Core:

```
dotnet add package Pi3.Infrastructure.Graph
```

## Registra il package Pi3.Infrastructure.Graph con IServiceCollection
Il package Pi3.Infrastructure.Graph supporta direttamente 
Microsoft.Extensions.DependencyInjection.Abstractions, pertanto è sufficiente utilizzare la sintassi seguente per registrare tutti i tipi e i servizi supportati.

```C#
// Registra l'infrastruttura Chilkat.
.AddInfrastructureGraph()
```

Questo registra:
- `IEventPublisher` come scoped
- `IFileConverterFactory` come scoped
- `IFileTextExtractorFactory` come scoped
- `IFileConverterService` come scoped
- `IEmailSenderService` come scoped
- `IEmailBoxScannerService` come scoped

## Package Pi3 inclusi
- [Pi3.Core](/Docs/Core.md)

## L'interfaccia IEmailSenderService

L'interfaccia **IEmailSenderService** definisce le modalità di invio di un messaggio di posta elettronica PEC o PEO ad uno o più destinatari e con o senza file allegati.

Per inviare un messaggio di posta elettronica:
```C#
  // Ottiene un riferimento al servizio
        var service = _serviceProvider.GetRequiredService<IEmailSenderService>();

try
{
    var result = await service.SendEmail(
       (configurations) =>
        {
            ((GraphSendEmailConfiguration)configurations).TenantId = "xxxx";
            ((GraphSendEmailConfiguration)configurations).ClientId = "xxxx";
            ((GraphSendEmailConfiguration)configurations).ClientSecret = "xxxx";
            ((GraphSendEmailConfiguration)configurations).MailBox = "xxxx@tenanttest.onmicrosoft.com";
        },
        new SendEmailInstructions()
        {
            Sender = new EmailSender()
            {
                Address = "mariorossi@tenanttest.onmicrosoft.com",
                DisplayName = "Mario Rossi"
            },
            To = new List<EmailRecipient>()
            {
                new EmailRecipient()
                {
                Address = "giuseppe.verdi@libero.it",
                DisplayName = "Giuseppe Verdi"
                }
            },
            Cc = new List<EmailRecipient>()
            {
                new EmailRecipient()
                {
                    Address = "mario.bianchi@gmail.com",
                    DisplayName = "Mario Bianchi"
                }
            },
            Bcc = new List<EmailRecipient>()
            {
                new EmailRecipient()
                {
                    Address = "giorgio.rossi@yahoo.com",
                    DisplayName = "Giorgio Rossi"
                }
            },        
            Subject = new Core.SeedWork.TextValue("Oggetto del messaggio di posta elettronica"),
            Body = new Core.SeedWork.TextValue("Testo del messaggio di posta elettronica"),
            BodyIsHtml = false,
            Attachments = new List<EmailContentAttachment>()
            {
                new EmailContentAttachment()
                {
                    FileName = "Allegato 1.pdf",
                    Content = new Byte[] { ... }, // Contenuto binario del documento
                    ContentType = "application/pdf"
                }
            }
        });

    Assert.True(!string.IsNullOrWhiteSpace(result.MessageId));
}
catch (GraphSendEmailPi3Exception cex)
{
    Assert.Fail(cex.LastErrorText);
}
```

## L'interfaccia IEmailBoxScannerService

L'interfaccia **IEmailBoxScannerService** definisce le modalità per interagire con una casella di posta elettronica leggendone i messaggi di posta ricevuti.

### Accesso ai messaggi di posta elettronica dalla mailbox tramite POP o IMAP

Il package implementa la classe **GraphEmailBoxScannerService**, che implementa **IEmailBoxScannerService**, per fornire l'accesso ai messaggi di posta ricevuti in una casella outlook tramite le api di Microsoft Graph.

Il metodo  **Scan** richiede in ingresso diverse informazioni per poter accedere alla casella di posta, che è possibile indicare mediante un oggetto StringDictionary.

L'implementazione di Graph supporta le seguenti informazioni:

Argomento | Descrizione |
--- | --- |
TenantId | Guid del tenant Microsoft. |
ClientId | ClientId dell'applicazione configurata per l'accesso. |
ClientSecret | ClientSecret dell'applicazione configurata per l'accesso. |
MailBox | Indirizzo email della mail box da processare. |
FolderToRead | Nome della cartella della mail box da analizzare. |

E' possibile implementare una funzione di callback richiamata per ogni email estratta dalla casella. L'implementatore può accedere, tramite l'oggetto Email, ai dati della mail ed eventuali allegati.

Per scansionare una casella di posta elettronica:
```C#

    try
    {
        // Ottiene un riferimento al servizio
        var service = _serviceProvider.GetRequiredService<IEmailBoxScannerService>();

        // Scansione della casella di posta
        await this._service.Scan(
                (configurations) =>
                {
                    ((GraphEmailBoxConfiguration)configurations).TenantId = "xxxxxxx";
                    ((GraphEmailBoxConfiguration)configurations).ClientId = "xxxxxxx";
                    ((GraphEmailBoxConfiguration)configurations).ClientSecret = "xxxxxxx";
                    ((GraphEmailBoxConfiguration)configurations).MailBox = "xxxx@tenanttest.onmicrosoft.com";
                    ((GraphEmailBoxConfiguration)configurations).FolderToRead = "da_protocollare";
                },
                (callback) =>
                    {
                        // callback.Email: Dati e allegati dell'email
                        // callback.Current: Indice dell'email corrente
                        // callback.Total: Numero totale di email estratte

                        // La funzione di callback restituisce un esito.
                        // - true: l'elaborazione ha avuto esito positivo, continua con la scansione della casella
                        // - false: l'elaborazione ha avuto esito negatio, annulla la scansione della casella
                        return true; 
                    }
                );

        Assert.Pass();
    }
    catch (BoxScannerPi3Exception ex)
    {
        Assert.Fail(ex.LastErrorText);
    }
```

Il metodo  **Parse** non è implementato.
