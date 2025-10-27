# Pi3.Infrastructure.Legacy

## Introduzione
Pi3.Infrastructure.Legacy raccoglie un insieme di package infrastrutturali in cui sono implementati i dettagli tecnici relativi all'utilizzo della piattaforma pre-esistente di PiTre, ovvero:
- la persistenza dei dati 
- l'accesso al file system del repository documentale

## Linguaggio, piattaforma e tool di sviluppo
C# 10, .NET 6, Visual Studio 2022.

## Codice sorgente

Nella tabella seguente sono descritti i progetti presenti all'interno del repository Pi3.Infrastructure.Legacy.

| Nome progetto  | Descrizione |
| ------------- | ------------- |
|[Pi3.Infrastructure.Legacy.DocumentFSRepository](/Docs/Pi3.Infrastructure.Legacy.DocumentFSRepository.md) | Progetto che implementa il repository del DocumentBlobAggregate per consentire l'accesso al file system del repository documentale.  |
|[Pi3.Infrastructure.Legacy.EF.Entities](/Docs/Pi3.Infrastructure.Legacy.EF.Entities.md) | Progetto che definisce tutti i dettagli per l'accesso alla base dati di PiTre mediante EntityFrameworkCore, quali le entities e l'interfaccia IPi3DbContext. |
|[Pi3.Infrastructure.Legacy.EF.Oracle](/Docs/Pi3.Infrastructure.Legacy.EF.Oracle.md) | Progetto che permette l'accesso alla base dati Oracle di PiTre implementando l'interfaccia IPi3DbContext. Include il package Oracle.EntityFrameworkCore. |
|[Pi3.Infrastructure.Legacy.EF.Services](/Docs/Pi3.Infrastructure.Legacy.EF.Services.md) | Progetto che implementa i servizi di Pi3 che necessitano di accedere alla base dati. |
|[Pi3.Infrastructure.Legacy.EF.AggregazioneDocumentaleAggregate](/Docs/Pi3.Infrastructure.Legacy.EF.AggregazioneDocumentaleAggregate.md) | Progetto che implementa il repository per la persistenza, con  Entity Framework, dell'aggregate AggregazioneDocumentale. |
|[Pi3.Infrastructure.Legacy.EF.DelegaAggregate](/Docs/Pi3.Infrastructure.Legacy.EF.DelegaAggregate.md) | Progetto che implementa il repository per la persistenza, con  Entity Framework, dell'aggregate Delega. |
|[Pi3.Infrastructure.Legacy.EF.DocumentoAmministrativoAggregate](/Docs/Pi3.Infrastructure.Legacy.EF.DocumentoAmministrativoAggregate.md) | Progetto che implementa il repository per la persistenza, con  Entity Framework, dell'aggregate DocumentoAmministrativo. |
|[Pi3.Infrastructure.Legacy.EF.KeywordAggregate](/Docs/Pi3.Infrastructure.Legacy.EF.KeywordAggregate.md) | Progetto che implementa il repository per la persistenza, con  Entity Framework, dell'aggregate Keyword. |
|[Pi3.Infrastructure.Legacy.EF.ListaDistribuzioneAggregate](/Docs/Pi3.Infrastructure.Legacy.EF.ListaDistribuzioneAggregate.md) | Progetto che implementa il repository per la persistenza, con  Entity Framework, dell'aggregate ListaDistribuzione. |
|[Pi3.Infrastructure.Legacy.EF.MezzoSpedizioneAggregate](/Docs/Pi3.Infrastructure.Legacy.EF.MezzoSpedizioneAggregate.md) | Progetto che implementa il repository per la persistenza, con  Entity Framework, dell'aggregate MezzoSpedizione. |
|[Pi3.Infrastructure.Legacy.EF.ModelloTrasmissioneAggregate](/Docs/Pi3.Infrastructure.Legacy.EF.ModelloTrasmissioneAggregate.md) | Progetto che implementa il repository per la persistenza, con  Entity Framework, dell'aggregate ModelloTrasmissione. |
|[Pi3.Infrastructure.Legacy.EF.NotaAggregate](/Docs/Pi3.Infrastructure.Legacy.EF.NotaAggregate.md) | Progetto che implementa il repository per la persistenza, con  Entity Framework, dell'aggregate Nota. |
|[Pi3.Infrastructure.Legacy.EF.NotaRFAggregate](/Docs/Pi3.Infrastructure.Legacy.EF.NotaRFAggregate.md) | Progetto che implementa il repository per la persistenza, con  Entity Framework, dell'aggregate NotaRF. |
|[Pi3.Infrastructure.Legacy.EF.OggettoAggregate](/Docs/Pi3.Infrastructure.Legacy.EF.OggettoAggregate.md) | Progetto che implementa il repository per la persistenza, con  Entity Framework, dell'aggregate Oggetto. |
|[Pi3.Infrastructure.Legacy.EF.PersonaCorrispondenteAggregate](/Docs/Pi3.Infrastructure.Legacy.EF.PersonaCorrispondenteAggregate.md) | Progetto che implementa il repository per la persistenza, con  Entity Framework, dell'aggregate PersonaCorrispondente. |
|[Pi3.Infrastructure.Legacy.EF.RagioneTrasmissioneAggregate](/Docs/Pi3.Infrastructure.Legacy.EF.RagioneTrasmissioneAggregate.md) | Progetto che implementa il repository per la persistenza, con  Entity Framework, dell'aggregate RagioneTrasmissione. |
|[Pi3.Infrastructure.Legacy.EF.RuoloCorrispondenteAggregate](/Docs/Pi3.Infrastructure.Legacy.EF.RuoloCorrispondenteAggregate.md) | Progetto che implementa il repository per la persistenza, con  Entity Framework, dell'aggregate RuoloCorrispondente. |
|[Pi3.Infrastructure.Legacy.EF.TrasmissioneAggregate](/Docs/Pi3.Infrastructure.Legacy.EF.TrasmissioneAggregate.md) | Progetto che implementa il repository per la persistenza, con  Entity Framework, dell'aggregate Trasmissione. |
|[Pi3.Infrastructure.Legacy.EF.UOCorrispondenteAggregate](/Docs/Pi3.Infrastructure.Legacy.EF.UOCorrispondenteAggregate.md) | Progetto che implementa il repository per la persistenza, con  Entity Framework, dell'aggregate UOCorrispondente. |
|Pi3.Infrastructure.Legacy.Test | Progetto contenente tutti i test unitari. |
|Pi3.Infrastructure.Legacy.sln | File di solution. |
