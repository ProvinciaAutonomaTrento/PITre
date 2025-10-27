# Pi3.Infrastructure.Tibco

Il package implementa i dettagli infrastrutturali per l'utilizzo dei servizi esposti dall'infrastruttura Tibco di TnDigit. 

Sono integrati i seguenti servizi:
- Sigillo elettronico 
- Verifica validità firme digitali 
- Firma remota
- Marca temporale

## Installa il package

Installa il package tramite NuGet Package Manager:

```
Install-Package Pi3.Infrastructure.Tibco
```

Oppure tramite l'interfaccia della riga di comando di .NET Core:

```
dotnet add package Pi3.Infrastructure.Tibco
```


## Registra il package Pi3.Infrastructure.Tibco con IServiceCollection
Il package Pi3.Infrastructure.Tibco supporta direttamente 
Microsoft.Extensions.DependencyInjection.Abstractions, pertanto è sufficiente utilizzare la sintassi seguente per registrare tutti i tipi e i servizi supportati.

```C#
// Registra il servizio di apposizione del sigillo elettronico 
services.AddSigilloElettronicoService(opt =>
{
    opt.ServiceUrl = "http://.../SigilloElettronico/api"; // Url dell'api Tibco
    opt.UserId = "******"; // UserId per l'accesso all'api Tibco  
    opt.UserPassword = "******"; // Password per l'accesso all'api Tibco  
    opt.Certificate = new System.Security.Cryptography.X509Certificates.X509Certificate2(new Byte[] {...}, "*******"); // Certificato X509 richiesto dall'api Tibco  
});
```
Questo registra:
- `ISigilloElettronicoService` come scoped

```C#
// Registra il servizio di verifica firma digitale
.AddFirmaDigitaleService(opt =>
{
    opt.ServiceUrl = "https://.../VerificaFirmeDigitali/Soap12";
    opt.Certificate = new System.Security.Cryptography.X509Certificates.X509Certificate2(new Byte[] {...}, "*******") // Certificato X509 richiesto dall'api Tibco                     
})
```
Questo registra:
- `IFirmaDigitaleService` come scoped

```C#
// Registra il servizio di verifica firma digitale 2
.AddFirmaDigitale2Service(opt =>
{
    opt.ServiceUrl = "https://.../SignDoc/VerSign";
    opt.Certificate = new System.Security.Cryptography.X509Certificates.X509Certificate2(new Byte[] {...}, "*******") // Certificato X509 richiesto dall'api Tibco                     
})
```
Questo registra:
- `IFirmaDigitale2Service` come scoped

```C#
// Registra il servizio di firma remota 
services.AddFirmaRemotaService(opt =>
{
    opt.ServiceUrl = "https://.../FirmaRemota/Soap12"; // Url dell'api Tibco
    opt.Certificate = new System.Security.Cryptography.X509Certificates.X509Certificate2(new Byte[] {...}, "*******") // Certificato X509 richiesto dall'api Tibco                     
});
```
Questo registra:
- `IFirmaRemotaService` come scoped

```C#
// Registra il servizio di firma remota 2
services.AddFirmaRemota2Service(opt =>
{
    opt.ServiceUrl = "https://.../signdoc/fremota"; // Url dell'api Tibco
    opt.Certificate = new System.Security.Cryptography.X509Certificates.X509Certificate2(new Byte[] {...}, "*******") // Certificato X509 richiesto dall'api Tibco                     
});
```
Questo registra:
- `IFirmaRemota2Service` come scoped


```C#
// Registra il servizio di marca temporale
services.AddMarcaTemporaleService(opt =>
{
    opt.ServiceUrl = "https://.../signdoc/marca"; // Url dell'api Tibco
    opt.Certificate = new System.Security.Cryptography.X509Certificates.X509Certificate2(new Byte[] {...}, "*******") // Certificato X509 richiesto dall'api Tibco                     
});
```
Questo registra:
- `IMarcaTemporaleService` come scoped

## Package Pi3 inclusi

- Nessuno

## L'interfaccia ISigilloElettronicoService

L'interfaccia **ISigilloElettronicoService** permette di firmare digitalmente (in PAdES) un file in formato PDF apponendovi, in aggiunta, un'etichetta testuale. E' utilizzato per apporre il testo della segnatura di protocollo ai documenti protocollati.

### Utilizzo del servizio di apposizione del sigillo elettronico

```C#
// Ottiene il riferimento al servizio
var service = this._serviceProvider.GetService<ISigilloElettronicoService>();

try
{
    var response = await service.SignPdf(new SignPDFType()
    {
        FileDaFirmare = new Byte[] { ... }, // Contenuto del file da firmare digitalmente
        CodiceAOOIPA = "ABC800E",
        CodiceEnteIPA = "p_TN",
        Apparence = new ApparenceType() // Indicare le coordinate del testo da apporre sul file PDF
        {
            leftx = 400,
            lefty = 812,
            location = null,
            page = 1,
            reason = null,
            rightx = 876,
            righty = 802,
            testo = "PAT/RFS133-15/01/2024-0033106" // Segnatura di protocollo
        }
    });

    Assert.IsTrue(response.FileFirmato != null); // Contenuto del file firmato digitalmente
}
catch (SigilloElettronicoPi3Exception ex)
{
    Assert.Fail(ex.Message);
}
```

## L'interfaccia IFirmaDigitaleService

L'interfaccia **IFirmaDigitaleService** permette di verificare la validità delle firme digitali apposte sul contenuto di un file fornito in input. Restituisce le informazioni relative ai firmatari e il contenuto del documento originale.

### Utilizzo del servizio di verifica firma

```C#
// Ottiene il riferimento al servizio
var service = serviceProvider.GetService<IFirmaDigitaleService>();

try
{
    var response = await service.Verifica(new Services.File.FirmaDigitale.ValueObjects.VerificaRequest()
        {
            FileFirmato = new Byte[] { ... }, // Contenuto binario del file firmato da verificare
            DataVerifica = DateTime.Now,
            ControlloFirmeAnnidate = true,
            FirmaSha1WithRSA = true,
            MarcaSha1WithRSA = true
        });

    Assert.IsTrue(response.Esito != null);
}
catch (FirmaDigitalePi3Exception ex)
{
    Assert.Fail(ex.Message);
}
```

## L'interfaccia IFirmaDigitale2Service

L'interfaccia **IFirmaDigitale2Service** permette di verificare la validità delle firme digitali apposte sul contenuto di un file fornito in input utilizzando le api della nuova infrastruttura Tibco. Restituisce le informazioni relative ai firmatari e il contenuto del documento originale.

### Utilizzo del servizio di verifica firma

```C#
// Ottiene il riferimento al servizio
var service = serviceProvider.GetService<IFirmaDigitale2Service>();

try
{
    var response = await service.Verifica(new Services.File.FirmaDigitale2.ValueObjects.VerificaRequest()
        {
            FileFirmato = new Byte[] { ... }, // Contenuto binario del file firmato da verificare
            DataVerifica = DateTime.Now,
            VerificaCompleta = false, // Per firma CAdES, indica se effettuare la verifica delle firme anche per i file annidati
            TipoVerifica = TipiVerifica.Esterna,
            ReturnFileOriginale = true,
            ReturnXmlCompleto = true
        });

    Assert.IsTrue(response.Esito != null);
}
catch (Services.File.FirmaDigitale2.Exceptions.FirmaDigitalePi3Exception ex)
{
    Assert.Fail(ex.Message);
}
```

## L'interfaccia IFirmaRemotaService

L'interfaccia **IFirmaRemotaService** permette di firmare digitalmente un documento mediante la firma remota HSM. Il client richiede l'OPT (indicando l'alias del certificato e il dominio). Successivamente, richiede la firma PAdES o CAdES inviando il contenuto del file da firmare digitalmente.

### Utilizzo del servizio di firma remota per richiedere un OPT

```C#
// Ottiene il riferimento al servizio
var service = this.serviceProvider.GetService<IFirmaRemotaService>();

try
{
    var response = await service.RichiestaOtp(new RichiestaOtpRequest()
    {
        AliasCertificato = "*****",
        DominioCertificato = "*****"
    });
    
    Assert.IsTrue(response.Status != null);
}
catch (FirmaRemotaPi3Exception ex)
{
    Assert.Fail(ex.Message);
}
```

### Utilizzo del servizio di firma remota per firmare in PAdES

```C#
// Ottiene il riferimento al servizio
var service = this.serviceProvider.GetService<IFirmaRemotaService>();

try
{
    var response = await service.FirmaPAdES(new FirmaPAdESRequest()
    {
        fileDaFirmare = new Byte[] { ... }, // Contenuto binario del file da firmare digitalmente
        AliasCertificato = "*****",
        DominioCertificato = ""*****",
        OtpFirma = "*****",
        MarcaTemporale = false,
        PinCertificato = "*****"
    });

    Assert.IsTrue(response.FileFirmato != null);
}
catch (FirmaRemotaPi3Exception ex)
{
    Assert.Fail(ex.Message);
}
```

## Utilizzo del servizio di firma remota per firmare in CAdES

```C#
// Ottiene il riferimento al servizio
var service = this.serviceProvider.GetService<IFirmaRemotaService>();

try
{
    var response = await service.FirmaCAdES(new FirmaCAdESRequest()
    {
        fileDaFirmare = new Byte[] { ... }, // Contenuto binario del file da firmare digitalmente            
        AliasCertificato = "*****",
        DominioCertificato = "*****",
        OtpFirma = "*****",
        MarcaTemporale = false,
        PinCertificato =  "*****",
        FirmaParallela = true
    });

    Assert.IsTrue(response.FileFirmato != null);
}
catch (FirmaRemotaPi3Exception ex)
{
    Assert.Fail(ex.Message);
}
```

## L'interfaccia IFirmaRemota2Service

L'interfaccia **IFirmaRemota2Service** permette di firmare digitalmente un documento mediante la firma remota HSM della nuova infrastruttura Tibco. Il client richiede l'OPT (indicando l'alias del certificato e il dominio). Successivamente, richiede la firma PAdES o CAdES inviando il contenuto del file da firmare digitalmente.

### Utilizzo del servizio di firma remota per richiedere un OPT

```C#
// Ottiene il riferimento al servizio
var service = this.serviceProvider.GetService<IFirmaRemota2Service>();

try
{
    var response = await service.RichiestaOtpREST(new RichiestaOtpRequest()
    {
        AliasCertificato = "*****",
        DominioCertificato = "*****"
    });
    
    Assert.IsTrue(response.Status != null);
}
catch (FirmaRemotaPi3Exception ex)
{
    Assert.Fail(ex.Message);
}
```

### Utilizzo del servizio di firma remota per firmare in PAdES

```C#
// Ottiene il riferimento al servizio
var service = this.serviceProvider.GetService<IFirmaRemota2Service>();

try
{
    var response = await service.FirmaPAdESREST(new FirmaPAdESRequest()
    {
        FilesDaFirmare = new List<FileDaFirmare>() // E' possibile firmare digitalmente più file con un'unica richiesta
        {
            new FileDaFirmare()
            {
                FileBase64 = new Byte[] { ... }, // Contenuto binario del file da firmare digitalmente
                FileName = "NomeFile.pdf"
            }
        },
        AliasCertificato = "*****",
        DominioCertificato = ""*****",
        OtpFirma = "*****",
        MarcaTemporale = false,
        PinCertificato = "*****",
        unzipOutput = true
    });

    Assert.True(
        response.FileFirmato != null
        && response.FileFirmato.Count == 1
        && response.DataOraFirma > DateTime.MinValue);
}
catch (FirmaRemotaPi3Exception ex)
{
    Assert.Fail(ex.Message);
}
```

### Utilizzo del servizio di firma remota per firmare in CAdES

```C#
// Ottiene il riferimento al servizio
var service = this.serviceProvider.GetService<IFirmaRemota2Service>();

try
{
    var response = await service.FirmaCAdESREST(new FirmaCAdESRequest()
    {
        FilesDaFirmare = new List<FileDaFirmare>() // E' possibile firmare digitalmente più file con un'unica richiesta
        {
            new FileDaFirmare()
            {
                FileBase64 = new Byte[] { ... }, // Contenuto binario del file da firmare digitalmente
                FileName = "NomeFile.pdf"
            }
        },
        AliasCertificato = "*****",
        DominioCertificato = ""*****",
        OtpFirma = "*****",
        MarcaTemporale = false,
        PinCertificato = "*****",
        unzipOutput = true
    });

    Assert.True(
        response.FileFirmato != null
        && response.FileFirmato.Count == 1
        && response.DataOraFirma > DateTime.MinValue);
}
catch (FirmaRemotaPi3Exception ex)
{
    Assert.Fail(ex.Message);
}
```

### Utilizzo del servizio di firma remota per visualizzare le informazioni su un certificato

```C#
// Ottiene il riferimento al servizio
var service = this.serviceProvider.GetService<IFirmaRemota2Service>();

try
{
    var response = await service.VisualizzaCertificatoREST(new VisualizzaCertificatoRequest()
    {
        AliasCertificato = "*****",
        DominioCertificato = ""*****",
    });

    Assert.IsTrue(response.Esito != null);
}
catch (FirmaRemotaPi3Exception ex)
{
    Assert.Fail(ex.Message);
}
```

## L'interfaccia IMarcaTemporaleService

L'interfaccia **IMarcaTemporaleService** permette di apporre le marche temporali tramite la nuova infrastruttura Tibco.

```C#
var service = this._serviceProvider.GetService<IMarcaTemporaleService>();

var response = await service.MarcaTsd(new MarcaTsdRequest()
{
    FileDaMarcare = new FileMarcatura()
    {
        FileBase64 = new Byte[] { ... }, // Contenuto binario del file da marcare
        FileName = "NomeFile.pdf"
    }
});

Assert.IsTrue(response.DataOraMarca != DateTime.MinValue
    && !string.IsNullOrWhiteSpace(response.SerialNumberMarca)
    && !string.IsNullOrWhiteSpace(response.TSAName)
    && response.FileMarcato != null);
```
