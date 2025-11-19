# Pi3.App.DocumentoAmministrativo.WebApi

## Introduzione
Nell'ambito della nuova architettura del PiTre in cloud, **Pi3.App.DocumentoAmministrativo.WebApi** è il nuovo microservizio che espone agli applicativi gestionali le funzionalità del Documento Amministrativo Informatico secondo le regole ed il formato stabiliti dalla normativa [AGID](https://www.agid.gov.it/sites/default/files/repository_files/allegato_5_metadati.pdf).

Nella figura seguente, la Web Application è rappresentata dal POD denominato **web-api-documentoamministrativo** all'interno del Cluster Kubernates : 

![Panoramica.drawio.png](https://gitlab.tndigit.it/tndigit/pitre/pi3.docs/-/blob/main/Images/Panoramica.drawio.png?ref_type=heads "Panoramica")

## Url Swagger

| Ambiente | Url Swagger |
| ---------| ------------|
| TEST | https://pitre-test.cloud-test-intra.tndigit.it/DocumentoAmministrativo/swagger/index.html |
| QUALITY | https://pitre-qual.cloud-qual-intra.tndigit.it/DocumentoAmministrativo/swagger/index.html |
| COLLAUDO | https://pitre-coll.cloud-qual-intra.tndigit.it/DocumentoAmministrativo/swagger/index.html |
| FORMAZIONE | https://pitre-formazione.cloud-intra.tn.it/DocumentoAmministrativo/swagger/index.html |
| PRODUZIONE | https://pitre.cloud-intra.tn.it/DocumentoAmministrativo/swagger/index.html |


## Linguaggio, piattaforma e tool di sviluppo
C# 10, .NET 6, Visual Studio 2022.

## Struttura interna dell'applicazione

L'applicazione è suddivisa nei seguenti progetti:

| Nome progetto  | Descrizione |
| ------------- | ------------- |
| Pi3.App.DocumentoAmministrativo.WebApi | Progetto che implementa l'applicazione Web con gli Api controller, i middleware, gli action filters, il layer applicativo CQRS, gli aspetti di registrazione delle librerie e dei package utilizzati. |
| Pi3.App.DocumentoAmministrativo.WebApi.Tests | Progetto che implementa i test unitari. |
| Pi3.App.DocumentoAmministrativo.WebApi.sln | File di solution. |

## Autenticazione
L'applicazione supporta l'autenticazione AAC, pertanto i client dovranno fornire nell'header di tutte le chiamate alle Api un access token valido.

## Elenco dei package Pi3 inclusi
- [Pi3.Infrastructure.AAC](https://gitlab.tndigit.it/tndigit/pitre/pi3.infrastructure.aac/-/blob/main/README.md?ref_type=heads)
- [Pi3.Infrastructure.Adobe](https://gitlab.tndigit.it/tndigit/pitre/pi3.infrastructure.adobe/-/blob/main/README.md?ref_type=heads)
- [Pi3.Infrastructure.Legacy.EF.Oracle](https://gitlab.tndigit.it/tndigit/pitre/pi3.infrastructure.legacy/-/blob/main/Docs/Pi3.Infrastructure.Legacy.EF.Oracle.md?ref_type=heads)
- [Pi3.Infrastructure.Legacy.EF.Services](https://gitlab.tndigit.it/tndigit/pitre/pi3.infrastructure.legacy/-/blob/main/Docs/Pi3.Infrastructure.Legacy.EF.Services.md?ref_type=heads)
- [Pi3.Infrastructure.Legacy.DocumentFSRepository](https://gitlab.tndigit.it/tndigit/pitre/pi3.infrastructure.legacy/-/blob/main/Docs/Pi3.Infrastructure.Legacy.EF.DocumentFSRepository.md?ref_type=heads)
- [Pi3.Infrastructure.Legacy.EF.DocumentoAmministrativoAggregate](https://gitlab.tndigit.it/tndigit/pitre/pi3.infrastructure.legacy/-/blob/main/Docs/Pi3.Infrastructure.Legacy.EF.DocumentoAmministrativoAggregate.md?ref_type=heads)
- [Pi3.Infrastructure.Legacy.EF.TrasmissioneAggregate](https://gitlab.tndigit.it/tndigit/pitre/pi3.infrastructure.legacy/-/blob/main/Docs/Pi3.Infrastructure.Legacy.EF.TrasmissioneAggregate.md?ref_type=heads)
