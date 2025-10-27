# Pi3.App.RubricaComune.WebApi

## Introduzione
Nell'ambito della nuova architettura del PiTre in cloud, **Pi3.App.RubricaComune.WebApi** è il nuovo microservizio per l'accesso alla rubrica comune.

Nella figura seguente, la Web Application è rappresentata dal POD denominato **web-api-legacy-rubricacomune** all'interno del Cluster Kubernates : 

![Panoramica.drawio.png](https://gitlab.tndigit.it/tndigit/pitre/pi3.docs/-/blob/main/Images/Panoramica.drawio.png?ref_type=heads "Panoramica")

## Url Swagger

| Ambiente | Url Swagger |
| ---------| ------------|
| TEST | https://pitre-test.cloud-test-intra.tndigit.it/RubricaComune/swagger/index.html |
| QUALITY | https://pitre-qual.cloud-qual-intra.tndigit.it/RubricaComune/swagger/index.html |
| COLLAUDO | https://pitre-coll.cloud-qual-intra.tndigit.it/RubricaComune/swagger/index.html |
| FORMAZIONE | https://pitre-formazione.cloud-intra.tn.it/RubricaComune/swagger/index.html |
| PRODUZIONE | https://pitre.cloud-intra.tn.it/RubricaComune/swagger/index.html |


## Linguaggio, piattaforma e tool di sviluppo
C# 10, .NET 6, Visual Studio 2022.

## Struttura interna dell'applicazione

L'applicazione è suddivisa nei seguenti progetti:

| Nome progetto  | Descrizione |
| ------------- | ------------- |
| Pi3.App.RubricaComune.WebApi | Progetto che implementa l'applicazione Web con gli Api controller, i middleware, gli action filters, il layer applicativo CQRS, gli aspetti di registrazione delle librerie e dei package utilizzati. |
| Pi3.App.RubricaComune.WebApi.Tests | Progetto che implementa i test unitari. |
| Pi3.App.RubricaComune.WebApi.sln | File di solution. |

## Autenticazione
L'applicazione supporta l'autenticazione AAC, pertanto i client dovranno fornire nell'header di tutte le chiamate alle Api un access token valido.

## Elenco dei package Pi3 inclusi
- [Pi3.Core](/Docs/Pi3.Core.md)
- [Pi3.Infrastructure.AAC](https://gitlab.tndigit.it/tndigit/pitre/pi3.infrastructure.aac/-/blob/main/README.md?ref_type=heads)
