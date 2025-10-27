# Pi3.App.Uploader.WebApi

## Introduzione
Nell'ambito della nuova architettura del PiTre in cloud, **Pi3.App.Uploader.WebApi** è il nuovo microservizio che consente l'upload di file, anche di grandi dimensioni, tramite un approccio a chunk (frammenti). 

Il processo di upload si articola in tre endpoint principali:

- Initialize: Avvia la procedura di upload generando un identificativo univoco per il file e preparando il sistema per ricevere i chunk.
- Upload Chunk: Consente di inviare una singola parte (chunk) del file, che viene associata all'upload corrente.
- Finalize: Conclude l'upload, validando i chunk ricevuti e ricomponendo il file finale.

Questo approccio garantisce una gestione efficiente e affidabile dei file di grandi dimensioni, riducendo i rischi di errori durante il trasferimento e facilitando la ripresa in caso di interruzioni.

Nella figura seguente, la Web Application è rappresentata dal POD denominato **web-api-legacy-uploader** all'interno del Cluster Kubernates : 

![Panoramica.drawio.png](https://gitlab.tndigit.it/tndigit/pitre/pi3.docs/-/blob/main/Images/Panoramica.drawio.png?ref_type=heads "Panoramica")

## Url Swagger

| Ambiente | Url Swagger |
| ---------| ------------|
| TEST | https://pitre-test.cloud-test-intra.tndigit.it/Uploader/swagger/index.html |
| QUALITY | https://pitre-qual.cloud-qual-intra.tndigit.it/Uploader/swagger/index.html |
| COLLAUDO | https://pitre-coll.cloud-qual-intra.tndigit.it/Uploader/swagger/index.html |
| FORMAZIONE | https://pitre-formazione.cloud-intra.tn.it/Uploader/swagger/index.html |
| PRODUZIONE | https://pitre.cloud-intra.tn.it/Uploader/swagger/index.html |


## Linguaggio, piattaforma e tool di sviluppo
C# 10, .NET 6, Visual Studio 2022.

## Struttura interna dell'applicazione

L'applicazione è suddivisa nei seguenti progetti:

| Nome progetto  | Descrizione |
| ------------- | ------------- |
| Pi3.App.Uploader.WebApi | Progetto che implementa l'applicazione Web con gli Api controller, i middleware, gli action filters, il layer applicativo CQRS, gli aspetti di registrazione delle librerie e dei package utilizzati. |
| Pi3.App.Uploader.WebApi.Tests | Progetto che implementa i test unitari. |
| Pi3.App.Uploader.WebApi.sln | File di solution. |

## Autenticazione
L'applicazione supporta l'autenticazione AAC, pertanto i client dovranno fornire nell'header di tutte le chiamate alle Api un access token valido.

## Elenco dei package Pi3 inclusi
- [Pi3.Core](/Docs/Pi3.Core.md)
- [Pi3.Infrastructure.AAC](https://gitlab.tndigit.it/tndigit/pitre/pi3.infrastructure.aac/-/blob/main/README.md?ref_type=heads)
- [Pi3.Infrastructure.Legacy.EF.Oracle]()
- [Pi3.Infrastructure.Legacy.EF.Services](https://gitlab.tndigit.it/tndigit/pitre/pi3.infrastructure.legacy.ef.services/-/blob/main/README.md?ref_type=heads)
- [Pi3.Infrastructure.Legacy.UploaderFS](https://gitlab.tndigit.it/tndigit/pitre/pi3.infrastructure.legacy.uploaderfs/-/blob/main/README.md?ref_type=heads)