# Pi3.Infrastructure.Chilkat

Il package permette di integrare i dettagli infrastrutturali per utilizzare Chilkat, una libreria di terze parti che fornisce strumenti per interagire con diversi protocolli internet. In questa versione, Chilkat è utilizzata per inviare messaggi di posta elettronica PEC o PEO implementando l'interfaccia IEmailSenderService.

## Installa il package

Installa il package tramite NuGet Package Manager:

```
Install-Package Pi3.Infrastructure.Chilkat
```

Oppure tramite l'interfaccia della riga di comando di .NET Core:

```
dotnet add package Pi3.Infrastructure.Chilkat
```

## Registra il package Pi3.Infrastructure.Chilkat con IServiceCollection
Il package Pi3.Infrastructure.Chilkat supporta direttamente 
Microsoft.Extensions.DependencyInjection.Abstractions, pertanto è sufficiente utilizzare la sintassi seguente per registrare tutti i tipi e i servizi supportati.

```C#
// Registra l'infrastruttura Chilkat.
services.AddInfrastructureChilkat(opt => opt.LicenseKey = "************");
```

Questo registra:
- `IEventPublisher` come scoped
- `IFileConverterFactory` come scoped
- `IFileTextExtractorFactory` come scoped
- `IFileConverterService` come scoped
- `IEmailSenderService` come scoped
- `IEmailBoxScannerService` come scoped
- `IPAdESService` come scoped

## Package Pi3 inclusi
- [Pi3.Core](/Docs/Core.md)

## L'interfaccia IEmailSenderService

L'interfaccia **IEmailSenderService** definisce le modalità di invio di un messaggio di posta elettronica PEC o PEO ad uno o più destinatari e con o senza file allegati.

Per inviare un messaggio di posta elettronica:
```C#
try
{
    // Ottiene un riferimento al servizio
    var service = _serviceProvider.GetRequiredService<IEmailSenderService>();

    var result = await service.SendEmail(
        new SmtpConfigurations()
        {
            Host = "smtp.libero.it",
            Port = 465,
            RequireSsl = true,
            UserName = "mario.rossi@libero.it",
            Password = "*****************"
        },
        new SendEmailInstructions()
        {
            Sender = new EmailSender()
            {
                Address = "mario.rossi@libero.it",
                DisplayName = "Mario Rossi
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

    // MessageId contiene l'identificativo univoco del messaggio di posta elettronica inviato
    Assert.True(!string.IsNullOrWhiteSpace(result.MessageId));
}
catch (ChilkatSendEmailPi3Exception cex)
{
    Assert.Fail(cex.LastErrorText);
}
```

## L'interfaccia IEmailBoxScannerService

L'interfaccia **IEmailBoxScannerService** definisce le modalità per interagire con una casella di posta elettronica leggendone i messaggi di posta ricevuti.

### Accesso ai messaggi di posta elettronica dalla mailbox tramite POP o IMAP

Il package implementa la classe **ChilkatEmailBoxScannerService**, che implementa **IEmailBoxScannerService**, per fornire l'accesso ai messaggi di posta ricevuti in una casella tramite standard POP o IMAP.

Il metodo  **Scan** richiede in ingresso diverse informazioni per poter accedere alla casella di posta, che è possibile indicare mediante un oggetto StringDictionary.

L'implementazione di Chilkat supporta le seguenti informazioni:

Argomento | Descrizione |
--- | --- |
EmailBoxTypeEnum | Tipologia di email: POP o IMAP. |
Host | Mail server host |
Port | Porta, dato numerico |
RequireSsl | true o false |
UserName | UserName per l'accesso alla casella di posta. |
Password | Password per l'accesso alla casella di posta. |

E' possibile implementare una funzione di callback richiamata per ogni email estratta dalla casella. L'implementatore può accedere, tramite l'oggetto Email, ai dati della mail ed eventuali allegati.

Per scansionare una casella di posta elettronica:
```C#
    var args = new StringDictionary();

    // Inizializzazione degli argomenti richiesti
    args.Add("EmailBoxTypeEnum", EmailBoxTypeEnum.POP.ToString());
    args.Add("Host", "mbox.cert.legalmail.it");
    args.Add("Port", "995");
    args.Add("RequireSsl", "true");
    args.Add("UserName", "******");
    args.Add("Password", "******");

    try
    {
        // Ottiene un riferimento al servizio
        var service = _serviceProvider.GetRequiredService<IEmailSenderService>();

        // Scansione della casella di posta
        await this._service.Scan(
                emailBoxConfigurations: 
                    new EmailBoxConfigurations 
                    { 
                        Arguments = args
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
    catch (ChilkatBoxScannerPi3Exception ex)
    {
        Assert.Fail(ex.LastErrorText);
    }
```

Il metodo  **Parse** permette di ottenere un oggetto **Email** a partire da un file .eml fornito in ingresso:

```C#
    try
    {
        // Ottiene un riferimento al servizio
        var service = _serviceProvider.GetRequiredService<IEmailSenderService>();

        var content = new Byte[] { ... }; // Caricamento contenuto del file .eml

        using (var stream = new MemoryStream(content));

        // Parsing del messaggio di posta
        var email = await this._service.Parse(stream);

        Assert.IsTrue(email != null);
    }
    catch (ChilkatBoxScannerPi3Exception ex)
    {
        Assert.Fail(ex.LastErrorText);
    }
```

## L'interfaccia IPAdESService

L'interfaccia **IPAdESService** fornisce funzioni di utilità per i file firmati digitalmente PAdES ed è implementata dalla classe  **ChilkatPAdESService**.

Il metodo  **IsPAdESFile** permette di verificare se il contenuto di un file fornito in ingresso è un file firmato PAdES:

```C#
    try
    {
        // Ottiene un riferimento al servizio
        var service = _serviceProvider.GetRequiredService<IPAdESService>();

        var content = new Byte[] { ... }; // Caricamento contenuto del file PAdES

        using (var stream = new MemoryStream(content));

        // Verifica se il file è firmato PAdES
        var isPAdES = await this._service.IsPAdESFile(stream);

        Assert.IsTrue(isPAdES);
    }
    catch (ChilkatPAdESServicePi3Exception ex)
    {
        Assert.Fail(ex.LastErrorText);
    }
```
