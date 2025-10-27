# Pi3.App.StampaRepertori.Batch

## Introduzione
L'applicazione ha lo scopo di generare le stampe giornaliere di Protocollo. 

L’espletamento del processo di stampa avviene mediante i seguenti passi per singola istanza PiTre:
-	reperimento di tutti i registri di repertorio presenti nell’istanza PiTre
-	per ciascun registro:
    -	sono reperiti tutti i dati da stampare, a partire dall’ultimo numero repertorio stampato
    -	chiusura temporanea del registro, per evitare inserimenti di nuovi repertori
    -	preparazione del report pdf (formato pdf/a) con inserimento dati estratti 
    -	creazione di un nuovo documento non protocollato ed acquisizione del report pdf appena creato
    -	contestuale invio del documento in conservazione
    -	riapertura del registro

## Linguaggio, piattaforma e tool di sviluppo
C# 10, .NET 6, Visual Studio 2022.

## Struttura interna dell'applicazione

L'applicazione è suddivisa nei seguenti progetti:

| Nome progetto  | Descrizione |
| ------------- | ------------- |
| Pi3.App.StampaRepertori.Batch | Progetto che implementa l'applicazione batch, il layer applicativo CQRS, gli aspetti di registrazione delle librerie e dei package utilizzati. |
| Pi3.App.StampaRepertori.Batch.Tests | Progetto che implementa i test unitari. |
| Pi3.App.StampaRepertori.Batch.sln | File di solution. |

## Autenticazione
L'applicazione impersonifica l'utenza di servizio configurata, nella base dati PiTre, per ogni registro repertorio.

## Modalità di avvio e schedulazione
Per avviare l'applicazione, è necessario fornire il nome dell'istanza come parametro da riga di comando (es. PAT_INSTANCE, TNDIGIT_INSTANCE, ecc.). La schedulazione è configurata tramite un cron job per l'esecuzione automatica quotidiana a partire dalle ore 22:00.

## Elenco dei package Pi3 inclusi
- [Pi3.Infrastructure.Chilkat](https://gitlab.tndigit.it/tndigit/pitre/pi3.infrastructure.chilkat/-/blob/main/README.md?ref_type=heads)
- [Pi3.Infrastructure.IText.ReportGenerator](https://gitlab.tndigit.it/tndigit/pitre/pi3.infrastructure.itext.reportgenerator/-/blob/main/README.md?ref_type=heads)
- [Pi3.Infrastructure.Legacy.DocumentFSRepository](https://gitlab.tndigit.it/tndigit/pitre/pi3.infrastructure.legacy.documentfsrepository/-/blob/main/README.md?ref_type=heads)
- [Pi3.Infrastructure.Legacy.EF.DocumentoAmministrativo](https://gitlab.tndigit.it/tndigit/pitre/pi3.infrastructure.legacy.ef.documentoamministrativo/-/blob/main/README.md?ref_type=heads)
- [Pi3.Infrastructure.Legacy.EF.Oracle](https://gitlab.tndigit.it/tndigit/pitre/pi3.infrastructure.legacy.ef.oracle/-/blob/main/README.md?ref_type=heads)
- [Pi3.Infrastructure.Legacy.EF.Services](https://gitlab.tndigit.it/tndigit/pitre/pi3.infrastructure.legacy.ef.services/-/blob/main/README.md?ref_type=heads)


