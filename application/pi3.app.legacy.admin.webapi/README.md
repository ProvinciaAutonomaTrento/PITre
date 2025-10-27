# Pi3.App.Legacy.Admin.WebApi

## Introduzione
Nell'ambito della nuova architettura del PiTre in cloud, **Pi3.App.Legacy.Admin.WebApi** è il microservizio dell'area Legacy utilizzato esclusivamente dal FrontEnd dell'amministrazione PiTre che espone il servizio SOAP DocsPaWS.asmx.

Nella figura seguente, la Web Application è rappresentata dal POD denominato **web-api-legacy-admin** all'interno del Cluster Kubernates : 

![Panoramica.drawio.png](https://gitlab.tndigit.it/tndigit/pitre/pi3.docs/-/blob/main/Images/Panoramica.drawio.png?ref_type=heads "Panoramica")

## Url
| Ambiente | Url asmx |
| ---------| ------------|
| TEST | https://pitre-test.cloud-test-intra.tndigit.it/LegacyAdmin/DocsPaWS.asmx |
| QUALITY | https://pitre-qual.cloud-qual-intra.tndigit.it/LegacyAdmin/DocsPaWS.asmx |
| COLLAUDO | https://pitre-coll.cloud-qual-intra.tndigit.it/LegacyAdmin/DocsPaWS.asmx |
| FORMAZIONE | https://pitre-formazione.cloud-intra.tn.it/LegacyAdmin/DocsPaWS.asmx |
| PRODUZIONE | https://pitre.cloud-intra.tn.it/LegacyAdmin/DocsPaWS.asmx |


## Linguaggio, piattaforma e tool di sviluppo
C# 10, .NET 6, Visual Studio 2022.

## Struttura interna dell'applicazione
Per ragioni di compatibilità con l'attuale FrontEnd dell'amministrazione PiTre, l'applicazione è stata migrata as-is dalla versione 4.7 di .NET Framework a .NET 6.
