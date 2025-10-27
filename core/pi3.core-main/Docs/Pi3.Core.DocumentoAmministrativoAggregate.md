# Pi3.Core.DocumentoAmministrativoAggregate
Il package  definisce l'aggregato DocumentoAmministrativoInformatico secondo le specifiche definite nelle linee guida [AGID](https://www.agid.gov.it/sites/default/files/repository_files/allegato_5_metadati.pdf).

## Installa il package

Installa il package tramite NuGet Package Manager:

```
Install-Package Pi3.Core.DocumentoAmministrativoAggregate
```

Oppure tramite l'interfaccia della riga di comando di .NET Core:

```
dotnet add package Pi3.Core.DocumentoAmministrativoAggregate
```

## Linguaggio, piattaforma e tool di sviluppo
C# 10, .NET 6, Visual Studio 2022.


## Elenco dei package Pi3 inclusi
- [Pi3.Core.DocumentAggregate](/Docs/Pi3.Core.DocumentAggregate.md)


## Crea un nuovo documento non protocollato

```C#
var aggregate = new DocumentoAmministrativo(
        "<idTenant>",
        DateTime.Now,
        new OggettoDelDocumento() 
        { 
            Descrizione = new TextValue(
                "Oggetto del documento amministrativo",
                new MultiLanguageTextValue("en", "Subject of the administrative document"),
                new MultiLanguageTextValue("de", "Betreff des Verwaltungsdokuments"))                
        });

// Aggiunta di una nuova classificazione
aggregate.AddClassification("<idClassification>");

// Persistenza dell'aggregate mediante repository
await _repository.Add(aggregate);

// Da qui, l'aggregate è aggiornato con le nuove informazioni ricavate dalla persistenza
```
> ##### Gestione testo multilingua
> L'oggetto `TextValue` permette di definire lo stesso testo in più lingue e può essere utilizzato in tutti i punti in cui è supportato all'interno degli aggregate.
> ```C#
> new TextValue(
>    "Oggetto del documento amministrativo",
>    new MultiLanguageTextValue("en", "Subject of the  administrative document"),
>    new MultiLanguageTextValue("de", "Betreff des Verwaltungsdokuments"))
> ```


## Crea un nuovo documento protocollato in entrata

```C#
var aggregate = new DocumentoAmministrativo(
        "<idTenant>",
        DateTime.Now,
        new OggettoDelDocumento() 
        { 
            Descrizione = new TextValue(
                "Oggetto del documento amministrativo",
                new MultiLanguageTextValue("en", "Subject of the administrative document"),
                new MultiLanguageTextValue("de", "Betreff des Verwaltungsdokuments"))                
        },
        TipologiaFlussoEnum.E);

// Aggiunta di una nuova classificazione
aggregate.AddClassification("<idClassification>");

// Aggiunta di un'aggregazione documentale di tipo fascicolo
aggregate.AddAggFascicolo("<idAggFascicolo>");

// Indica che l'aggregazione documentale deve essere principale
.AssignClassificationOrAggAsPrincipale(aggregate.Aggregazioni[0]);

// Assegna il mezzo di spedizione
aggregate.AssignMezzoSpedizione("<idMezzoSpedizione>");

// Assegna un mittente già esistente in rubrica 
aggregate.AssignMittente(new Mittente("<idMittente>"));

// Inserisce un mittente multiplo PersonaFisica non esistente in rubrica
aggregate.AddMittenteMultiplo(new Mittente(
        new PF()
        {
                Cognome = "<Cognome>",
                Nome = "<Nome>",
                CodiceFiscale = "<CodiceFiscale>"
        }));

// Inserisce un mittente multiplo PersonaGiuridica non esistente in rubrica
aggregate.AddMittenteMultiplo(new Mittente(
        new PG()
        {
                DenominazioneOrganizzazione = new TextValue("<DenominazioneOrganizzazione>"),
                DenominazioneUfficio = new TextValue("DenominazioneUfficio"),
                CodiceFiscalePartitaIva = "<CodiceFiscalePartitaIva>",
                IndirizziDigitaliDiRiferimento = new List<string>()
                {
                        "IndirizziDigitaliDiRiferimento@domain.com",
                        "IndirizziDigitaliDiRiferimento2@domain.com",
                        "https://www.IndirizziDigitaliDiRiferimento.com"
                }
        }));

// Assegna le eventuali informazioni del protocollo dell'amministrazione mittente
aggregate.AssignProtocolloMittente(new ProtocolloMittente()
        {
                Data = DateTime.Now.AddDays(-1),
                DataArrivo = DateTime.Now,
                Segnatura = "<Segnatura>"
        });

// Richiede che il documento, in fase di persistenza, venga registrato all'interno del registro di protocollo
aggregate.RichiediRegistrazione(new DatiRichiestaRegistrazione()
        {
        IdRegistro = "<IdRegistro>",
        Predisponi = false // Indicare se creare il documento come predisposto
        });

// Persistenza dell'aggregate mediante repository
await _repository.Add(aggregate);

// Da qui, l'aggregate è aggiornato con le nuove informazioni ricavate dalla persistenza
```

## Crea un nuovo documento protocollato in uscita

```C#
var aggregate = new DocumentoAmministrativo(
        "<idTenant>",
        DateTime.Now,
        new OggettoDelDocumento() 
        { 
            Descrizione = new TextValue(
                "Oggetto del documento amministrativo",
                new MultiLanguageTextValue("en", "Subject of the administrative document"),
                new MultiLanguageTextValue("de", "Betreff des Verwaltungsdokuments"))                
        },
        TipologiaFlussoEnum.U);

// Aggiunta di una nuova classificazione
aggregate.AddClassification("<idClassification>");

// Aggiunta di un'aggregazione documentale di tipo fascicolo
aggregate.AddAggFascicolo("<idAggFascicolo>");

// Indica che l'aggregazione documentale deve essere principale
.AssignClassificationOrAggAsPrincipale(aggregate.Aggregazioni[0]);

// Assegna un mittente già esistente in rubrica 
aggregate.AssignMittente(new Mittente("<idMittente>"));

// Assegna un destinatario già esistente in rubrica
aggregate.AddDestinatario(new Destinatario("<IdDestinatario>"));

// Inserisce un destinatario PersonaFisica non esistente in rubrica
aggregate.AddDestinatario(new Destinatario(
        new PF()
        {
                Cognome = "<Cognome>",
                Nome = "<Nome>",
                CodiceFiscale = "<CodiceFiscale>"
        }));

// Inserisce un destinatario in conoscenza PersonaGiuridica non esistente in rubrica
aggregate.AddDestinatarioCc(new Destinatario(
        new PG()
        {
                DenominazioneOrganizzazione = new TextValue("<DenominazioneOrganizzazione>"),
                DenominazioneUfficio = new TextValue("DenominazioneUfficio"),
                CodiceFiscalePartitaIva = "<CodiceFiscalePartitaIva>",
                IndirizziDigitaliDiRiferimento = new List<string>()
                {
                        "IndirizziDigitaliDiRiferimento@domain.com",
                        "IndirizziDigitaliDiRiferimento2@domain.com",
                        "https://www.IndirizziDigitaliDiRiferimento.com"
                }
        }));

// Richiede che il documento, in fase di persistenza, venga registrato all'interno del registro di protocollo
aggregate.RichiediRegistrazione(new DatiRichiestaRegistrazione()
        {
        IdRegistro = "<IdRegistro>",
        Predisponi = false // Indicare se creare il documento come predisposto
        });

// Persistenza dell'aggregate mediante repository
await _repository.Add(aggregate);

// Da qui, l'aggregate è aggiornato con le nuove informazioni ricavate dalla persistenza
```


## Crea un nuovo documento allegato

L'allegato è un documento non protocollato allegato ad un documento esistente. Non può esistere al di fuori del suo documento padre, dal quale eredita tutti i diritti di  visibilità. Non può essere né classificato, né fascicolato, né profilato.
L'indicazione del documento padre può essere fornita solamente nel costruttore. 

```C#
var aggregate = new DocumentoAmministrativo(
        "<idTenant>",
        DateTime.Now,
        new OggettoDelDocumento() 
        { 
            Descrizione = new TextValue(
                "Documento allegato",
                new MultiLanguageTextValue("en", "Attached document"),
                new MultiLanguageTextValue("de", "Anhang"))                
        },
        null,
        null,
        new IdDoc()
        {       
                // Identificativo del documento padre
                Identiticativo = "<Identificativo>"
        }););


// Persistenza dell'aggregate mediante repository
await _repository.Add(aggregate);

// Da qui, l'aggregate è aggiornato con le nuove informazioni ricavate dalla persistenza
```
## Gestire le versioni

## Associare un file ad una versione

## Gestire gli allegati

## Gestire le Classificazioni
Il documento amministrativo può essere classificato più volte ma non quando si presenta una delle seguenti condizioni:
- è in stato consolidato (Livello 2) 
- se non è un allegato
- è stato protocollato

### Aggiungere una classificazione

Indicare l'identificativo della classifica ed eventualmente la sua descrizione

``` C#
aggregate.AddClassification(
"<idClassification>",
 new TextValue("<description>"));
```

### Rimuovere una classificazione

Indicare l'identificativo della classifica da rimuovere.

``` C#
aggregate.Remove("<idClassification>");
```

### Elencare le classificazioni

``` C#
var classifications = aggregate.Classifications;
```

## Gestire le Aggregazioni
Le aggregazioni documentali possono essere di 3 tipi:
- Fascicolo
- SerieDocumentale
- SerieDiFascicoli

Il documento amministrativo può essere inserito in più aggregazioni eccetto quando si presenta una delle seguenti condizioni:
- se non è un allegato


### Aggiungere un'aggregazione

Indicare l'identificativo dell'aggregazione ed eventualmente la sua descrizione

``` C#
aggregate.AddAggFascicolo(
"<idFascicolo>",
 new TextValue("<description>"));

 aggregate.AddAggSerieDocumentale(
"<idSerieDocumentale>",
 new TextValue("<description>"));

 aggregate.AddAggSerieDiFascicoli(
"<idSerieDiFascicoli>",
 new TextValue("<description>"));
```

### Rimuovere un'aggregazione

Indicare l'identificativo dell'aggregazione da rimuovere.

``` C#
aggregate.RemoveAggFascicolo("<idFascicolo>");

aggregate.RemoveAggSerieDocumentale("<idSerieDocumentale>");

aggregate.AddAggSerieDiFascicoli("<idSerieDiFascicoli>");
```

### Elencare le aggregazioni

``` C#
var aggregazioni = aggregate.Aggregazioni;
```
## Consolidamento

### Consolidare il documento
Il consolidamento del documento (operazione irreversibile) comporta restrizioni alle modifiche del documento. 

Esistono due livelli di consolidamento:
- Livello 1: Non è possibile acquisire nuove versioni
- Livello 2: I metadati non sono più modificabili

``` C#
aggregate.Consolida(new Consolidamento()
        {
                Autore = new Autore("<idAutore>"),
                Data = DateTime.Now,
                Stato = StatiConsolidamentoEnum.Livello1
        });
```

### Ottenere lo stato di consolidamento del documento
``` C#
var consolidamento = aggregate.Consolidamento;
```
## Modificare l'oggetto del documento

## Gestire le parole chiave del documento

### Aggiungere una parola chiave

### Rimuovere una parola chiave

### Elencare le parole chiave 

## Gestire i profili documentali

## Gestire il blocco / sblocco

## Gestire le membership

## Gestire le catene documentali

## Gestire i soggetti del documento
mittenti, mittentimultipli, destinatari, eccetto

## Inserire il documento nel cestino










