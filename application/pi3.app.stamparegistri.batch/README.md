# Pi3.App.StampaRegistri.Batch

## Introduzione
L'applicazione ha lo scopo di generare le stampe giornaliere di Protocollo. 

L’espletamento del processo di stampa avviene mediante i seguenti passi per singola istanza PiTre:
-	reperimento di tutte le amministrazioni presenti nell’istanza PiTre
-	per ciascuna amministrazione, sono reperiti i registri di protocollo
-	per ciascun registro:
    -	è effettuata la chiusura temporanea, per evitare la creazione di nuovi protocolli
    -	reperimento dati da stampare:
        -	sono reperiti i dati dei nuovi protocolli da stampare: se si tratta della prima stampa sono considerati tutti i protocolli creati, in caso contrario sono considerati tutti i protocolli creati a partire dall’ultimo numero di protocollo stampato
        -	sono reperite le eventuali variazioni ai protocolli già stampati. Le variazioni considerate sono: annullamenti, modifica oggetto, storicizzazione corrispondenti, nuove versioni, file acquisiti dopo l’ultima stampa, nuove versioni allegati
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
| Pi3.App.StampaRegistri.Batch | Progetto che implementa l'applicazione batch, il layer applicativo CQRS, gli aspetti di registrazione delle librerie e dei package utilizzati. |
| Pi3.App.StampaRegistri.Batch.Tests | Progetto che implementa i test unitari. |
| Pi3.App.StampaRegistri.Batch.sln | File di solution. |

## Autenticazione
L'applicazione impersonifica un'utenza di servizio presente in ogni amministrazione. Lo username dell'utenza deve avere il suffisso "STAMPAREG".

## Modalità di avvio e schedulazione
Per avviare l'applicazione, è necessario fornire il nome dell'istanza come parametro da riga di comando (es. PAT_INSTANCE, TNDIGIT_INSTANCE, ecc.). La schedulazione è configurata tramite un cron job per l'esecuzione automatica quotidiana a partire dalle ore 02:00.

## Elenco dei package Pi3 inclusi
- [Pi3.Infrastructure.Chilkat](https://gitlab.tndigit.it/tndigit/pitre/pi3.infrastructure.chilkat/-/blob/main/README.md?ref_type=heads)
- [Pi3.Infrastructure.IText.ReportGenerator](https://gitlab.tndigit.it/tndigit/pitre/pi3.infrastructure.itext.reportgenerator/-/blob/main/README.md?ref_type=heads)
- [Pi3.Infrastructure.Legacy.DocumentFSRepository](https://gitlab.tndigit.it/tndigit/pitre/pi3.infrastructure.legacy.documentfsrepository/-/blob/main/README.md?ref_type=heads)
- [Pi3.Infrastructure.Legacy.EF.DocumentoAmministrativo](https://gitlab.tndigit.it/tndigit/pitre/pi3.infrastructure.legacy.ef.documentoamministrativo/-/blob/main/README.md?ref_type=heads)
- [Pi3.Infrastructure.Legacy.EF.Oracle](https://gitlab.tndigit.it/tndigit/pitre/pi3.infrastructure.legacy.ef.oracle/-/blob/main/README.md?ref_type=heads)
- [Pi3.Infrastructure.Legacy.EF.Services](https://gitlab.tndigit.it/tndigit/pitre/pi3.infrastructure.legacy.ef.services/-/blob/main/README.md?ref_type=heads)


