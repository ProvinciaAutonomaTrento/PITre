# Pi3.App.Conservazione.Batch

## Introduzione
L'applicazione si occupa della preparazione dei pacchetti di versamento e l’invio al conservatore.

L’espletamento del processo di versamento avviene mediante i seguenti passi per singola istanza PiTre:
-	reperimento delle amministrazioni presenti nell’istanza PiTre
-	per ciascuna amministrazione:
    -	Sono reperite le policy di invio in conservazione configurate e attive (le policy sono definite nel tool di amministrazione, in area “Conservazione”, dove l’amministratore può definire diversi criteri di filtro per estrarre documenti, fascicoli o stampe da inviare in conservazione)
    -	Per ciascuna policy:
        -	in base ai filtri definiti per la policy, è reperito l’elenco dei documenti, fascicoli o stampe da inviare in conservazione
        -	per ciascun documento / fascicolo / stampa reperito:
            -	viene estesa la visibilità al ruolo responsabile della conservazione
            -	viene creato un pacchetto di versamento ed inviato al conservatore
            -	viene aggiornato lo stato di invio in conservazione (successo o fallimento)
        -	generazione di un report pdf (formato pdf/a) contenente l’esito dell’invio in conservazione per tutti i documenti estratti dalla policy
        -	creazione di un nuovo documento non protocollato con allegato il report pdf generato e contestuale trasmissione al ruolo responsabile conservazione

L’attività di versamento dei report semestrali non segue la schedulazione appena descritta ma è eseguita manualmente nei mesi di gennaio e luglio.


## Linguaggio, piattaforma e tool di sviluppo
C# 10, .NET 6, Visual Studio 2022.

## Struttura interna dell'applicazione

L'applicazione è suddivisa nei seguenti progetti:

| Nome progetto  | Descrizione |
| ------------- | ------------- |
| Pi3.App.Conservazione.Batch | Progetto che implementa l'applicazione batch, il layer applicativo CQRS, gli aspetti di registrazione delle librerie e dei package utilizzati. |
| Pi3.App.Conservazione.Batch.Tests | Progetto che implementa i test unitari. |
| Pi3.App.Conservazione.Batch.sln | File di solution. |

## Autenticazione
L'applicazione impersonifica un'utenza di servizio configurata, nella base dati PiTre, per ciascuna amministrazione.

## Modalità di avvio e schedulazione
Per avviare l'applicazione, è necessario fornire i seguenti parametri da riga di comando:
- Parametro 1: il nome dell'istanza (es. PAT_INSTANCE, TNDIGIT_INSTANCE, ecc.)
- Parametro 2: modalità di avvio. Valori ammessi: 0, 1. (0: Esecuzione delle policy configurate in amministrazione, 1: Invio report giornalieri di errore).
La schedulazione è configurata tramite un cron job per l'esecuzione automatica quotidiana a partire dalle ore 09:00 (modalità 1) e 21:00 (modalità 0).

## Elenco dei package Pi3 inclusi
- [Pi3.Infrastructure.Chilkat](https://gitlab.tndigit.it/tndigit/pitre/pi3.infrastructure.chilkat/-/blob/main/README.md?ref_type=heads)
- [Pi3.Infrastructure.IText.ReportGenerator](https://gitlab.tndigit.it/tndigit/pitre/pi3.infrastructure.itext.reportgenerator/-/blob/main/README.md?ref_type=heads)
- [Pi3.Infrastructure.Legacy.DocumentFSRepository](https://gitlab.tndigit.it/tndigit/pitre/pi3.infrastructure.legacy.documentfsrepository/-/blob/main/README.md?ref_type=heads)
- [Pi3.Infrastructure.Legacy.EF.DocumentoAmministrativo](https://gitlab.tndigit.it/tndigit/pitre/pi3.infrastructure.legacy.ef.documentoamministrativo/-/blob/main/README.md?ref_type=heads)
- [Pi3.Infrastructure.Legacy.EF.Oracle](https://gitlab.tndigit.it/tndigit/pitre/pi3.infrastructure.legacy.ef.oracle/-/blob/main/README.md?ref_type=heads)
- [Pi3.Infrastructure.Legacy.EF.Services](https://gitlab.tndigit.it/tndigit/pitre/pi3.infrastructure.legacy.ef.services/-/blob/main/README.md?ref_type=heads)


