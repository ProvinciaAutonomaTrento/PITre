using System;
using System.ServiceModel;
using P3SBCLib.Contracts;

namespace P3SBCLib
{
    /// <summary>
    /// Servizi creati ad hoc per permettere al sistema SBC di integrarsi 
    /// con i servizi di PITRE nel modo più semplice e performante possibile. 
    /// I servizi che la classe definisce permettono l'accesso semplificato 
    /// ad alcuni dei servizi supportati dell'area funzionale dei fascicoli e dei documenti di PITRE. 
    /// Come prerequisito è necessario che in PITRE siano già stati creati i tipi fascicoli e i tipi documenti 
    /// contenenti gli attributi personalizzati necessari per creare le corrispondenza 
    /// tra fascicolo PITRE - pratica SBC e tra documento PITRE - provvedimento SBC. 
    /// </summary>
    /// <remarks>
    /// I servizi non implementano alcun meccanismo di autenticazione utente verso PITRE, 
    /// questo per evitare un accoppiamento del sistema SBC con le logiche di sessione utente proprie di PITRE. 
    /// Pertanto, il sistema SBC si autenticherà ai servizi presentando un opportuno certificato X509. 
    /// Ciò garantirà l'accesso ai metodi solo ai richiedenti dei servizi che forniranno le credenziali 
    /// corrente tramite il certificato prescelto. 
    /// </remarks>
    [ServiceContract(Namespace = "http://valueteam.com/CustomServices/P3SBC")]
    public interface IP3SBCServices
    {
        /// <summary>
        /// Servizio per il download del file associato ad un documento in PITRE
        /// </summary>
        /// <param name="userIdRichiedente">UserID dell'utente PITRE che esegue la richiesta</param>
        /// <param name="numeroDocumento">Numero del documento
        /// <remarks>
        /// In PITRE, corrisponde all'identificativo univoco del documento
        /// </remarks>
        /// </param>
        /// <returns>Oggetto contenente i metadati e il file associato al documento</returns>
        /// <exception cref="P3SBCLib.Contracts.P3OperationFault">
        /// Errore nell'esecuzione dell'operazione
        /// </exception>
        [OperationContract]
        [FaultContract(typeof(P3OperationFault))]
        Documento DownloadDocumento(string userIdRichiedente, string numeroDocumento);

        /// <summary>
        /// Servizio per l'upload del file associato ad un documento in PITRE 
        /// </summary>
        /// <param name="userIdRichiedente">UserID dell'utente PITRE che esegue la richiesta</param>
        /// <param name="numeroDocumento">Numero del documento
        /// <remarks>
        /// In PITRE, corrisponde all'identificativo univoco del documento
        /// </remarks>
        /// </param>
        /// <param name="documento">Oggetto contenente il file da associare al documento</param>
        /// <exception cref="P3SBCLib.Contracts.P3OperationFault">
        /// Errore nell'esecuzione dell'operazione
        /// </exception>
        [OperationContract]
        [FaultContract(typeof(P3OperationFault))]
        void UploadDocumento(string userIdRichiedente, string numeroDocumento, Documento documento);

        /// <summary>
        /// Servizio per la ricerca dei documenti in PITRE
        /// </summary>
        /// <param name="userIdRichiedente">UserID dell'utente PITRE che esegue la richiesta</param>
        /// <param name="filtriRicerca">Criteri per filtrare i risultati della ricerca</param>
        /// <param name="criteriPaginazione">Opzionale. Consente di specificare i criteri per paginare i risultati della ricerca. Se non definito, non sarà applicata alcuna paginazione ai dati</param>
        /// <returns>Esito della ricerca documenti effettuata</returns>
        /// <exception cref="P3SBCLib.Contracts.P3OperationFault">
        /// Errore nell'esecuzione dell'operazione
        /// </exception>
        [OperationContract]
        [FaultContract(typeof(P3OperationFault))]
        ElencoInfoDocumenti GetDocumenti(string userIdRichiedente, FiltroRicercaDocumenti filtriRicerca, CriteriPaginazione criteriPaginazione);

        /// <summary>
        /// Servizio per il reperimento dei dati di un documento in SBC a partire dal numero
        /// </summary>
        /// <param name="userIdRichiedente">UserID dell'utente PITRE che esegue la richiesta </param>
        /// <param name="numeroDocumento">Numero del documento
        /// <remarks>
        /// In PITRE, corrisponde all'identificativo univoco del documento
        /// </remarks>
        /// </param>
        /// <param name="downloadContent">
        /// Indica se caricare contestualmente al servizio il file associato al documento
        /// </param>
        /// <returns>Metadati del documento</returns>
        /// <exception cref="P3SBCLib.Contracts.P3OperationFault">
        /// Errore nell'esecuzione dell'operazione
        /// </exception>
        [OperationContract]
        [FaultContract(typeof(P3OperationFault))]
        InfoDocumento GetDocumento(string userIdRichiedente, string numeroDocumento, bool downloadContent);

        /// <summary>
        /// Servizio per l'associazione di un documento in una pratica
        /// </summary>
        /// <param name="userIdRichiedente">UserID dell'utente PITRE che esegue la richiesta</param>
        /// <param name="numeroPratica">Numero della pratica</param>
        /// <param name="numeroDocumento">Numero del documento</param>
        /// <exception cref="P3SBCLib.Contracts.P3OperationFault">
        /// Errore nell'esecuzione dell'operazione
        /// </exception>
        [OperationContract]
        [FaultContract(typeof(P3OperationFault))]
        void AddDocumentoPratica(string userIdRichiedente, string numeroPratica, string numeroDocumento);

        /// <summary>
        /// Rimozione di un documento da una pratica
        /// </summary>
        /// <param name="userIdRichiedente">UserID dell'utente PITRE che esegue la richiesta</param>
        /// <param name="numeroPratica">Numero della pratica</param>
        /// <param name="numeroDocumento">Numero del documento</param>
        /// <exception cref="P3SBCLib.Contracts.P3OperationFault">
        /// Errore nell'esecuzione dell'operazione
        /// </exception>
        [OperationContract]
        [FaultContract(typeof(P3OperationFault))]
        void RemoveDocumentoPratica(string userIdRichiedente, string numeroPratica, string numeroDocumento);

        /// <summary>
        /// Servizio per la ricerca delle pratiche in SBC (fascicoli in PITRE).
        /// </summary>
        /// <param name="userIdRichiedente">UserID dell'utente PITRE che esegue la richiesta</param>
        /// <param name="filtriRicerca">Criteri per filtrare i risultati della ricerca. E' necessario specificare almeno l'attributo di ricerca "Classificazione"</param>
        /// <param name="criteriPaginazione">Opzionale. Consente di specificare i criteri per paginare i risultati della ricerca. Se non definito, non sarà applicata alcuna paginazione ai dati</param>
        /// <returns>Esito della ricerca pratiche effettuata</returns>
        /// <exception cref="P3SBCLib.Contracts.P3OperationFault">
        /// Errore nell'esecuzione dell'operazione
        /// </exception>
        [OperationContract]
        [FaultContract(typeof(P3OperationFault))]
        ElencoPratiche GetPratiche(string userIdRichiedente, FiltriRicercaPratiche filtriRicerca, CriteriPaginazione criteriPaginazione);

        /// <summary>
        /// Servizio per il reperimento di una Pratica in SBC
        /// </summary>
        /// <param name="userIdRichiedente">UserID dell'utente PITRE che esegue la richiesta</param>
        /// <param name="numeroPratica">Numero della pratica in SBC</param>
        /// <remarks>
        /// Corrisponde ad un Fascicolo in PITRE
        /// </remarks>
        /// <returns>Metadati della pratica richiesta</returns>
        /// <exception cref="P3SBCLib.Contracts.P3OperationFault">
        /// Errore nell'esecuzione dell'operazione
        /// </exception>
        [OperationContract]
        [FaultContract(typeof(P3OperationFault))]
        Pratica GetPratica(string userIdRichiedente, string numeroPratica);

        /// <summary>
        /// Servizio per il reperimento di una o più Pratiche in SBC
        /// </summary>
        /// <param name="userIdRichiedente">UserID dell'utente PITRE che esegue la richiesta</param>
        /// <param name="numeriPratica">
        /// Identificativi di una o più pratiche SBC per cui si vogliono reperire i metadati
        /// </param>
        /// <remarks>
        /// Corrisponde ad un Fascicolo in PITRE
        /// </remarks>
        /// <returns>Metadati delle pratiche richieste</returns>
        /// <exception cref="P3SBCLib.Contracts.P3OperationFault">
        /// Errore nell'esecuzione dell'operazione
        /// </exception>
        [OperationContract()]
        [FaultContract(typeof(P3OperationFault))]
        Pratica[] GetPraticheDaNumeri(string userIdRichiedente, string[] numeriPratica);

        /// <summary>
        /// Servizio per l'inserimento di un fascicolo in PITRE (pratica in SBC) di una particolare tipologia già configurata 
        /// </summary>
        /// <param name="userIdRichiedente">UserID dell'utente PITRE che esegue la richiesta</param>
        /// <param name="pratica">Dati della pratica SBC da inserire come fascicolo in PITRE</param>
        /// <returns>Metadati della pratica inserita</returns>
        /// <exception cref="P3SBCLib.Contracts.P3OperationFault">
        /// Errore nell'esecuzione dell'operazione
        /// </exception>
        [OperationContract]
        [FaultContract(typeof(P3OperationFault))]
        Pratica InsertPratica(string userIdRichiedente, Pratica pratica);

        /// <summary>
        /// Servizio per l'aggiornamento dei dati di un fascicolo esistente in PITRE (pratica in SBC)
        /// </summary>
        /// <param name="userIdRichiedente">UserID dell'utente PITRE che esegue la richiesta</param>
        /// <param name="pratica">Dati della pratica SBC da modificare nel corrispondente fascicolo in PITRE</param>
        /// <returns>Metadati della pratica aggiornata</returns>
        /// <exception cref="P3SBCLib.Contracts.P3OperationFault">
        /// Errore nell'esecuzione dell'operazione
        /// </exception>
        [OperationContract]
        [FaultContract(typeof(P3OperationFault))]
        Pratica UpdatePratica(string userIdRichiedente, Pratica pratica);

        /// <summary>
        /// Servizio per la ricerca degli utenti attivi in PITRE
        /// </summary>
        /// <param name="filtriRicerca">
        /// Criteri per filtrare i risultati della ricerca
        /// <remarks>
        /// Come filtro obbligatorio è necessario definire almeno una Classificazione,
        /// in quanto saranno reperiti solamente gli utenti che ne avranno la visibilità 
        /// </remarks>
        /// </param>
        /// <param name="criteriPaginazione">Opzionale. Consente di specificare i criteri per paginare i risultati della ricerca. Se non definito, non sarà applicata alcuna paginazione ai dati</param>
        /// <returns>Esito della ricerca utenti effettuata</returns>
        /// <exception cref="P3SBCLib.Contracts.P3OperationFault">
        /// Errore nell'esecuzione dell'operazione
        /// </exception>
        [OperationContract]
        [FaultContract(typeof(P3OperationFault))]
        ElencoUtenti GetUtenti(FiltriRicercaUtenti filtriRicerca, CriteriPaginazione criteriPaginazione);
    }
}
