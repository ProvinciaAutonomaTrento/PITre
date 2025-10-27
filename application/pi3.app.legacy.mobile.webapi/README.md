# Pi3.App.Legacy.Mobile.WebApi

## Introduzione
Nell'ambito della nuova architettura del PiTre in cloud, **Pi3.App.Legacy.Mobile.WebApi** è il microservizio dell'area Legacy utilizzato esclusivamente dall'app mobile di PiTre.

Nella figura seguente, la Web Application è rappresentata dal POD denominato **web-api-legacy-mobile** all'interno del Cluster Kubernates: 

![Panoramica.drawio.png](https://gitlab.tndigit.it/tndigit/pitre/pi3.docs/-/blob/main/Images/Panoramica.drawio.png?ref_type=heads "Panoramica")

## Url
| Ambiente | Url |
| ---------| ------------|
| TEST | https://pitre-test.cloud-test.tndigit.it/LegacyPisMobile |
| QUALITY |  |
| COLLAUDO |  |
| FORMAZIONE |  |
| PRODUZIONE |  |


## Linguaggio, piattaforma e tool di sviluppo
C# 10, .NET 6, Visual Studio 2022.

## Struttura interna dell'applicazione
Per ragioni di compatibilità con l'attuale app mobile di PiTre, l'applicazione è stata migrata as-is dalla versione 4.7 di .NET Framework a .NET 6.
