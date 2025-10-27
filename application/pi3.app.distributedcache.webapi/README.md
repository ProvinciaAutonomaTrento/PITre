# Pi3.App.DistributedCache.WebApi

## Introduzione
Nell'ambito della nuova architettura del PiTre in cloud, **Pi3.App.DistributedCache.WebApi** è il microservizio che permette di gestire la cache distribuita Redis anche dagli applicativi esterni al cluster Kubernetes (poiché il server Redis non è esposto).

E' utilizzato in particolare dal FrontEnd PiTre per gestire in cache alcune informazioni, quali il token di autenticazione AAC e la tabella degli SwitchService.

Nella figura seguente, la Web Application è rappresentata dal POD denominato **web-api-distributedcache** all'interno del Cluster Kubernates: 

![Panoramica.drawio.png](https://gitlab.tndigit.it/tndigit/pitre/pi3.docs/-/blob/main/Images/Panoramica.drawio.png?ref_type=heads "Panoramica")

## Url Swagger

| Ambiente | Url Swagger |
| ---------| ------------|
| TEST | https://pitre-test.cloud-test-intra.tndigit.it/DistributedCache/swagger/index.html |
| QUALITY | https://pitre-qual.cloud-qual-intra.tndigit.it/DistributedCache/swagger/index.html |
| COLLAUDO | https://pitre-coll.cloud-qual-intra.tndigit.it/DistributedCache/swagger/index.html |
| FORMAZIONE | https://pitre-formazione.cloud-intra.tn.it/DistributedCache/swagger/index.html |
| PRODUZIONE | https://pitre.cloud-intra.tn.it/DistributedCache/swagger/index.html |


## Linguaggio, piattaforma e tool di sviluppo
C# 10, .NET 6, Visual Studio 2022.

## Struttura interna dell'applicazione

L'applicazione è suddivisa nei seguenti progetti:

| Nome progetto  | Descrizione |
| ------------- | ------------- |
| Pi3.App.DistributedCache.WebApi | Progetto che implementa l'applicazione Web con gli Api controller, i middleware, gli action filters, gli aspetti di registrazione delle librerie e dei package utilizzati. |
| Pi3.App.DistributedCache.WebApi.Tests | Progetto che implementa i test unitari. |
| Pi3.App.DistributedCache.WebApi.sln | File di solution. |

## Autenticazione
L'applicazione supporta l'autenticazione Basic, pertanto i client dovranno fornire nell'header di tutte le chiamate alle Api un token valido in base64 nel formato {userName}:{password}. L'applicazione verifica le credenziali fornite con le informazioni presenti in Vault.

## Progetto Pi3.App.DistributedCache.WebApi
Nel progetto sono implementate le Api controller, i middleware, gli action filters e gli aspetti di registrazione delle librerie e dei package utilizzati dall'applicazione.

### Api controller

L'applicazione definisce le seguenti Api controller:
- ValuesController
- KeysController

#### ValuesController
Espone endpoint per gestire le informazioni in cache, ovvero inserimento, modifica e cancellazione.```

#### KeysController
Espone endpoint per enumerare le chiavi inserite in cache.

## Elenco dei package Pi3 inclusi
- [Pi3.Core](/Docs/Pi3.Core.md)