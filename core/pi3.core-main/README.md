# Pi3.Core

## Introduzione
Parte centrale dell'architettura nel nuovo sistema PiTre, i package Pi3.Core implementano le politiche aziendali, con le relative regole e validazioni, tramite gli artefatti descritti dall'approccio [Domain Driven Design](https://github.com/gg-daddy/ebooks/blob/master/Eric%20Evans%202003%20-%20Domain-Driven%20Design%20-%20Tackling%20Complexity%20in%20the%20Heart%20of%20Software.pdf) (DDD). 

Definisce anche interfacce di servizio trasversali non riconducibili a concetti di dominio.

> **Nota**: In Pi3.Core non sono implementati gli aspetti infrastrutturali e tecnologici, come ad esempio la persistenza o l'utilizzo di protocolli di rete. Tali dettagli sono implementati nei package con prefisso Pi3.Infrastructure.*.

### Domain Driven Design
Approccio allo sviluppo per risolvere problemi complessi mappando i concetti di un dominio aziendale in un modello software (modello di dominio).​

**Aggregate**: Insieme di oggetti logicamente correlati tra loro e racchiusi all’interno di un confine applicativo o sotto-dominio deliminato (Buonded Context). Un esempio di aggregate, può essere il contesto del documento amministrativo informatico.​

**AggregateRoot**: Oggetto di dominio principale dell’Aggregate, ha una propria identità (Id) e garantisce la consistenza dei dati e del comportamento tra tutti gli oggetti dell’intero insieme. Le operazioni riguardanti gli oggetti devono tutte transitare dall’AggregateRoot.​

**Entity**: Oggetto di dominio, ha una propria identità (Id)​

**ValueObject**: E’ un oggetto che non ha un’identità propria ma solo una serie di attributi immutabili​

**DomainEvent**: È un oggetto che racchiude al suo interno le informazioni relative ad un evento significativo avvenuto nell’Aggregate. Serve per notificare gli accadimenti agli altri aggregate senza legarli tra loro.​

**Repository**: Implementa la logica di persistenza dell’intero Aggregate fornendo un’estrazione simile ad una collection (es. Exists, Get, Add, Update, Delete). Deve garantire il reperimento e l’aggiornamento dei dati dell’Aggregate nella sua interezza (transazionalità). Ogni aggregate definisce l'interfaccia del proprio Repository, la persistenza vera e propria è implementata nei progetti del layer *Infrastracture*.

## Linguaggio, piattaforma e tool di sviluppo
C# 10, .NET 6, Visual Studio 2022.

## Codice sorgente

Nella tabella seguente sono descritti i progetti presenti all'interno del repository Pi3.Core.

| Nome progetto  | Descrizione |
| ------------- | ------------- |
|[Pi3.Core](/Docs/Core.md) | Progetto contenente tutte le astrazioni utilizzate dagli altri package. Dal Package Pi3.Core derivano tutti gli altri package.  |
| [Pi3.Core.ElementAggregate](/Docs/Pi3.Core.ElementAggregate.md)  | Implementa Element, l'aggregato astratto utilizzato come base per tutti gli altri aggregates. Definisce gli attributi principali quali Id, IdTenant, Name, Description, i profili dinamici e le parole chiave. Implementa  gli aspetti che permettono di definire la validità dello stato dell'aggregate. |
| [Pi3.Core.ContentElementAggregate](/Docs/Pi3.Core.ContentElementAggregate.md) | Estende ElementAggregate implementando  ContentElement, l'aggregate astratto che aggiunge la gestione dei permissions, delle classifications, degli elementi concatenati, le reservations.  |
| [Pi3.Core.DocumentAggregate](/Docs/Pi3.Core.DocumentAggregate.md)  | Estende ContentElementAggregate implementando  Document, l'aggregate che rappresenta la parte dei metadati del documento con le sue caratteristiche principali, tra cui il versioning ed il collegamento con i file associati.  |
| [Pi3.Core.DocumentBlobAggregate](/Docs/Pi3.Core.DocumentBlobAggregate.md) | Estende ContentElementAggregate implementando  DocumentBlob, l'aggregate che rappresenta il contenuto del file associato ai metadati del documento.  Tra le sue caratteristiche la possibilità di caricare il contenuto di un file a partire da uno stream o direttamente da un path file e il calcolo dell'hash (SHA256, SHA512).  |
| [Pi3.Core.DocumentoAmministrativoAggregate](/Docs/Pi3.Core.DocumentoAmministrativoAggregate.md) | Estende DocumentAggregate implementando  DocumentoAmministrativo, l'aggregate che attribuisce al documento tutte le funzionalità del documento amministrativo secondo quanto definito dalla normativa AGID (all. 5).  Tra le sue caratteristiche, la definizione del tipo di flusso (E, I, U), la gestione dell'oggetto del documento, i soggetti (mittenti, desintari, ecc.), la protocollazione / repertoriazione, i documenti allegati, le aggregazioni collegate, le note. |
| [Pi3.Core.AggregazioneDocumentaleAggregate](/Docs/Pi3.Core.AggregazioneDocumentaleAggregate.md) | Estende ContentElementAggregate implementando  AggregazioneDocumentale, l'aggregate che definisce il concetto di aggregazione secondo quanto definito dalla normativa AGID (all. 5).  Tra le sue caratteristiche, la definizione del tipo di aggregazione (Fascicolo, SerieDocumentale, SerieFascicoli), apertura e chiusura, il procedimento amministrativo, i documenti contenuti, le note. |
| [Pi3.Core.TrasmissioneAggregate](/Docs/Pi3.Core.TrasmissioneAggregate.md) | Estende ElementAggregate, definisce l'aggregato Trasmissione mediante il quale è possibile trasmettere documenti amministrativi o aggregazioni documentali a utenti o gruppi destinatari interni all'ogranigramma PiTre. |
| [Pi3.Core.RagioneTrasmissioneAggregate](/Docs/Pi3.Core.RagioneTrasmissioneAggregate.md) | Estende ElementAggregate, definisce l'aggregato RagioneTrasmissione mediante il quale è possibile amministrare le ragioni utilizzate per trasmettere documenti amministrativi o aggregazioni documentali. |
| [Pi3.Core.ModelloTrasmissioneAggregate](/Docs/Pi3.Core.ModelloTrasmissioneAggregate.md) | Estende ElementAggregate, definisce l'aggregato ModelloTrasmissione mediante il quale è possibile amministrare i modelli pre-confezionati utilizzati per trasmettere documenti amministrativi o aggregazioni documentali. |
| [Pi3.Core.DelegaAggregate](/Docs/Pi3.Core.DelegaAggregate.md) | Estende ElementAggregate implementando  Delega, l'aggregate per la gestione completa delle sostituzioni. |
| [Pi3.Core.KeywordAggregate](/Docs/Pi3.Core.KeywordAggregate.md) | Estende ElementAggregate, definisce l'aggregato Keyword per la gestire completa delle parole chiave utilizzate per indicizzare documenti amministrativi e aggregazioni documentali.  |
| [Pi3.Core.NotaAggregate](/Docs/Pi3.Core.NotaAggregate.md) | Estende ElementAggregate, definisce l'aggregato Nota per la gestione completa delle note utilizzate dai documenti amministrativi e aggregazioni documentali. |
| [Pi3.Core.NotaRFAggregate](/Docs/Pi3.Core.NotaRFAggregate.md) | Estende ElementAggregate, definisce l'aggregato NotaRF per la gestione completa dell'anagrafica delle note associate agli RF. Tali note sono a loro volta inserite nelle Note dei documenti amministrativi e aggregazioni documentali. |
| [Pi3.Core.OggettoAggregate](/Docs/Pi3.Core.OggettoAggregate.md) | Estende ElementAggregate, definisce l'aggregato Oggetto per gestire l'anagrafica dell'oggettario. Gli oggetti sono utilizzabili nei documenti amministrativi e aggregazioni documentali. |
| [Pi3.Core.MezzoSpedizioneAggregate](/Docs/Pi3.Core.MezzoSpedizioneAggregate.md) | Estende ElementAggregate, definisce l'aggregato MezzoSpedizione, metadato utilizzato dai documenti protocollati in entrata.  |
| [Pi3.Core.CorrispondenteAggregate](/Docs/Pi3.Core.CorrispondenteAggregate.md)  | Estende ElementAggregate, definisce l'aggregato astratto per Corrispondente come ulteriore base per gestire i corrispondenti esterni all'organigramma PiTre. |
| [Pi3.Core.PersonaCorrispondenteAggregate](/Docs/Pi3.Core.PersonaCorrispondenteAggregate.md) | Estende CorrispondenteAggregate per definire l'aggregato PersonaCorrispondente, il quale permette la gestione di un corrispondente esterno all'organigramma PiTre di tipo persona fisica. |
| [Pi3.Core.RuoloCorrispondenteAggregate](/Docs/Pi3.Core.RuoloCorrispondenteAggregate.md) | Estende CorrispondenteAggregate per definire l'aggregato RuoloCorrispondente, il quale permette la gestione di un corrispondente esterno all'organigramma PiTre di tipo ruolo. |
| [Pi3.Core.UOCorrispondenteAggregate](/Docs/Pi3.Core.UOCorrispondenteAggregate.md) | Estende CorrispondenteAggregate per definire l'aggregato UOCorrispondente, il quale permette la gestione di un corrispondente esterno all'organigramma PiTre di tipo Ufficio / Unità Organizzativa. |
| [Pi3.Core.ListaDistribuzioneAggregate](/Docs/Pi3.Core.ListaDistribuzioneAggregate.md) | Estende ElementAggregate per definire l'aggregato ListaDistribuzione, il quale permette di definire, attribuendogli un nome, un soggetto aggregate per corrispondenti di tipo persona, gruppo o ufficio. Tali soggetti possono essere utilizzati come destinatari per trasmissioni di documenti amministrativi o aggregazioni documentali. |
| Pi3.Core.Tests  | Progetto contenente tutti i test unitari  |
| Pi3.Core.sln  | File di solution  |
