using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaWS.Conservazione.PacchettiVersamento.Services
{
    /// <summary>
    /// Definizione dei servizi web dei i pacchetti di versamento della conservazione.
    /// I servizi permetteranno agli enti non aderenti al protocollo federato di poter
    /// utilizzare le caratteristiche del sistema di conservazione.
    /// </summary>
    public interface IServices
    {
        #region Servizi per l'autenticazione

        /// <summary>
        /// Servizio per l'autenticazione e connessione dell'utente al sistema di conservazione
        /// </summary>
        /// <param name="request">
        /// Dati della richiesta di autenticazione
        /// </param>
        /// <returns>
        /// Il servizio potrà restituire i seguenti codici di errore se l'operazione non andrà a buon fine:
        /// - PARAMETRO_MANCANTE
        ///     Parametro obbligatorio non fornito    
        ///     
        /// - AUTENTICAZIONE_FALLITA
        ///     Il nome utente utente la password specificati non sono validi
        ///     
        /// - UTENTE_NON_ABILITATO
        ///     L'utente non risulta abilitato per l'utilizzo dei servizi di versamento
        /// </returns>
        LogIn.LogInResponse LogIn(LogIn.LogInRequest request);

        ///// <summary>
        ///// Servizio per la disconnessione dell'utente dal sistema di conservazione
        ///// </summary>
        ///// <param name="request"></param>
        ///// <returns></returns>
        //LogOut.LogOutResponse LogOut(LogOut.LogOutRequest request);

        #endregion

        #region Servizi per la gestione dell'istanza di conservazione

        /// <summary>
        /// Servizio per la creazione di una nuova istanza di conservazione
        /// </summary>
        /// <param name="request">
        /// Dati della richiesta
        /// </param>
        /// <returns>
        /// Il servizio potrà restituire i seguenti codici di errore se l'operazione non andrà a buon fine:
        /// - PARAMETRO_MANCANTE
        ///     Parametro obbligatorio non fornito    
        /// 
        /// - TOKEN_NON_VALIDO
        ///     Il token di autenticazione fornito non è valido o scaduto
        ///     
        /// - ERRORE_NON_GESTITO: 
        ///     Si è verificato un errore non previsto tra quelli censiti
        /// </returns>
        CreateIstanza.CreateIstanzaResponse CreateIstanza(CreateIstanza.CreateIstanzaRequest request);

        /// <summary>
        /// Servizio per la cancellazione di un'istanza di conservazione
        /// </summary>
        /// <param name="request">
        /// Dati della richiesta
        /// </param>
        /// <returns>
        /// Il servizio potrà restituire i seguenti codici di errore se l'operazione non andrà a buon fine:
        /// - PARAMETRO_MANCANTE
        ///     Parametro obbligatorio non fornito    
        /// 
        /// - TOKEN_NON_VALIDO
        ///     Il token di autenticazione fornito non è valido o scaduto
        ///     
        /// - ISTANZA_NON_TROVATA
        ///     Istanza di conservazione inesistente
        ///     
        /// - ISTANZA_NON_CANCELLABILE: 
        ///     L'istanza di conservazione non è cancellabile in quanto già inviata al Centro Servizi
        ///     
        /// - ERRORE_NON_GESTITO: 
        ///     Si è verificato un errore non previsto tra quelli censiti
        /// </returns>
        /// <remarks>
        /// Il servizio può essere eseguito solamente se l'istanza non è ancora stata inviata al Centro Servizi
        /// </remarks>
        DeleteIstanza.DeleteIstanzaResponse DeleteIstanza(DeleteIstanza.DeleteIstanzaRequest request);

        /// <summary>
        /// Servizio per il reperimento dei dati di un'istanza di conservazione
        /// </summary>
        /// <param name="request">
        /// Dati della richiesta
        /// </param>
        /// <returns>
        /// Il servizio potrà restituire i seguenti codici di errore se l'operazione non andrà a buon fine:
        /// - PARAMETRO_MANCANTE
        ///     Parametro obbligatorio non fornito    
        /// 
        /// - TOKEN_NON_VALIDO
        ///     Il token di autenticazione fornito non è valido o scaduto
        ///     
        /// - ISTANZA_NON_TROVATA
        ///     Istanza di conservazione inesistente
        ///     
        /// - ERRORE_NON_GESTITO: 
        ///     Si è verificato un errore non previsto tra quelli censiti
        /// </returns>
        GetIstanza.GetIstanzaResponse GetIstanza(GetIstanza.GetIstanzaRequest request);

        /// <summary>
        /// Servizio per il reperimento dei dati dell'istanza di conservazione da inviare al Centro Servizi
        /// </summary>
        /// <param name="request">
        /// Dati della richiesta
        /// </param>
        /// <returns>
        /// Il servizio potrà restituire i seguenti codici di errore se l'operazione non andrà a buon fine:
        /// - PARAMETRO_MANCANTE
        ///     Parametro obbligatorio non fornito    
        /// 
        /// - TOKEN_NON_VALIDO
        ///     Il token di autenticazione fornito non è valido o scaduto
        ///     
        /// - ISTANZA_NON_TROVATA
        ///     Istanza di conservazione inesistente
        ///     
        /// - ERRORE_NON_GESTITO: 
        ///     Si è verificato un errore non previsto tra quelli censiti
        /// </returns>
        GetIstanzaDaInviare.GetIstanzaDaInviareResponse GetIstanzaDaInviare(GetIstanzaDaInviare.GetIstanzaDaInviareRequest request);

        /// <summary>
        /// Servizio per il reperimento e ricerca dei dati delle istanze di conservazione
        /// </summary>
        /// <param name="request">
        /// Dati della richiesta
        /// </param>
        /// <returns>
        /// Il servizio potrà restituire i seguenti codici di errore se l'operazione non andrà a buon fine:
        /// - PARAMETRO_MANCANTE
        ///     Parametro obbligatorio non fornito    
        /// 
        /// - TOKEN_NON_VALIDO
        ///     Il token di autenticazione fornito non è valido o scaduto
        ///     
        /// - ERRORE_NON_GESTITO: 
        ///     Si è verificato un errore non previsto tra quelli censiti
        /// </returns>
        GetIstanze.GetIstanzeResponse GetIstanze(GetIstanze.GetIstanzeRequest request);

        /// <summary>
        /// Servizio per l'inserimento di un documento nell'istanza di conservazione corrente
        /// </summary>
        /// <param name="request">
        /// Dati della richiesta
        /// </param>
        /// <returns>
        /// Il servizio potrà restituire i seguenti codici di errore se l'operazione non andrà a buon fine:
        /// - PARAMETRO_MANCANTE
        ///     Parametro obbligatorio non fornito    
        /// 
        /// - TOKEN_NON_VALIDO
        ///     Il token di autenticazione fornito non è valido o scaduto
        ///     
        /// - DOCUMENTO_NON_TROVATO
        ///     Documento non trovato.
        ///     
        /// - INSERIMENTO_IN_ISTANZA_FALLITO
        ///     Errore nell'inserimento del documento nell'istanza di conservazione corrente
        ///     
        /// - ERRORE_NON_GESTITO: 
        ///     Si è verificato un errore non previsto tra quelli censiti
        /// </returns>
        AddDocumentoInIstanza.AddDocumentoInIstanzaResponse AddDocumentoInIstanza(AddDocumentoInIstanza.AddDocumentoInIstanzaRequest request);

        /// <summary>
        /// Servizio per l'inserimento di un fascicolo nell'istanza di conservazione corrente
        /// </summary>
        /// <param name="request">
        /// Dati della richiesta
        /// </param>
        /// <returns>
        /// Il servizio potrà restituire i seguenti codici di errore se l'operazione non andrà a buon fine:
        /// - PARAMETRO_MANCANTE
        ///     Parametro obbligatorio non fornito    
        /// 
        /// - TOKEN_NON_VALIDO
        ///     Il token di autenticazione fornito non è valido o scaduto
        ///     
        /// - FASCICOLO_NON_TROVATO
        ///     Fascicolo non trovato.
        ///     
        /// - INSERIMENTO_IN_ISTANZA_FALLITO
        ///     Errore nell'inserimento del fascicolo nell'istanza di conservazione corrente
        ///     
        /// - ERRORE_NON_GESTITO: 
        ///     Si è verificato un errore non previsto tra quelli censiti
        /// </returns>
        AddFascicoloInIstanza.AddFascicoloInIstanzaResponse AddFascicoloInIstanza(AddFascicoloInIstanza.AddFascicoloInIstanzaRequest request);

        #endregion

        #region Servizi per la gestione dei documenti in un'istanza di conservazione

        /// <summary>
        /// Servizio per l'inserimento di un documento in un'istanza di conservazione
        /// </summary>
        /// <param name="request">
        /// Dati della richiesta
        /// </param>
        /// <returns>
        /// Il servizio potrà restituire i seguenti codici di errore se l'operazione non andrà a buon fine:
        /// - PARAMETRO_MANCANTE
        ///     Parametro obbligatorio non fornito
        /// 
        /// - ISTANZA_NON_TROVATA
        ///     Istanza di conservazione inesistente
        ///     
        /// - PROFILO_NON_TROVATO
        ///     Profilo del documento non trovato
        /// 
        /// - CREAZIONE_DOCUMENTO
        ///     Errore nella creazione del documento da inserire in conservazione
        ///     
        /// - UPLOAD_FILE
        ///     Errore nell'upload del file
        /// 
        /// - CONTENUTO_FILE_NON_CONFORME
        ///     Errore relativo al controllo di validazione effettuato sul contenuto del file rispetto al suo formato
        /// 
        /// - TOKEN_NON_VALIDO
        ///     Il token di autenticazione fornito non è valido o scaduto
        /// </returns>
        InsertDocumento.InsertDocumentoResponse InsertDocumento(InsertDocumento.InsertDocumentoRequest request);

        ///// <summary>
        ///// Servizio per l'upload di un file ad un documento in un'istanza di conservazione
        ///// </summary>
        ///// <param name="request">
        ///// Dati della richiesta
        ///// </param>
        ///// <returns>
        ///// Il servizio potrà restituire i seguenti codici di errore se l'operazione non andrà a buon fine:
        ///// - PARAMETRO_MANCANTE
        /////     Parametro obbligatorio non fornito
        ///// 
        ///// - DOCUMENTO_NON_TROVATO
        /////     Documento non trovato
        /////     
        ///// - UPLOAD_FILE
        /////     Errore nell'upload del file
        ///// 
        ///// - CONTENUTO_FILE_NON_CONFORME
        /////     Errore relativo al controllo di validazione effettuato sul contenuto del file rispetto al suo formato
        ///// 
        ///// - TOKEN_NON_VALIDO
        /////     Il token di autenticazione fornito non è valido o scaduto
        ///// </returns>
        //UploadFile.UploadFileResponse UploadFile(UploadFile.UploadFileRequest request);

        /// <summary>
        /// Servizio per il reperimento di un documento in un'istanza di conservazione
        /// </summary>
        /// <param name="request">
        /// Dati della richiesta
        /// </param>
        /// <returns>
        /// Il servizio potrà restituire i seguenti codici di errore se l'operazione non andrà a buon fine:
        /// - PARAMETRO_MANCANTE
        ///     Parametro obbligatorio non fornito
        /// </returns>
        GetDocumento.GetDocumentoResponse GetDocumento(GetDocumento.GetDocumentoRequest request);

        /// <summary>
        /// Servizio per il reperimento dei documenti presenti in un'istanza di conservazione
        /// </summary>
        /// <param name="request">
        /// Dati della richiesta
        /// </param>
        /// <returns>
        /// Il servizio potrà restituire i seguenti codici di errore se l'operazione non andrà a buon fine:
        /// - PARAMETRO_MANCANTE
        ///     Parametro obbligatorio non fornito
        ///     
        /// - TOKEN_NON_VALIDO
        ///     Il token di autenticazione fornito non è valido o scaduto
        /// 
        /// - ISTANZA_NON_TROVATA
        ///     Istanza di conservazione inesistente
        /// </returns>
        /// <remarks>
        /// Per motivi di performance, il servizio non restituisce né i campi profilati né il contenuto dei file.
        /// Utilizzare il metodo GetDocumento per ottenere tali informazioni.
        /// </remarks>
        GetDocumenti.GetDocumentiResponse GetDocumenti(GetDocumenti.GetDocumentiRequest request);

        /// <summary>
        /// Servizio per la cancellazione di un documento da un'istanza di conservazione
        /// </summary>
        /// <param name="request">
        /// Dati della richiesta
        /// </param>
        /// <returns>
        /// Il servizio potrà restituire i seguenti codici di errore se l'operazione non andrà a buon fine:
        /// - PARAMETRO_MANCANTE
        ///     Parametro obbligatorio non fornito
        ///     
        /// - TOKEN_NON_VALIDO
        ///     Il token di autenticazione fornito non è valido o scaduto
        /// 
        /// - ISTANZA_NON_TROVATA
        ///     Istanza di conservazione inesistente
        ///     
        /// - DOCUMENTO_NON_CANCELLABILE
        ///     Il documento non può essere rimosso dall'istanza di conservazione
        ///     
        /// - ERRORE_NON_GESTITO
        ///     Si è verificato un errore non previsto tra quelli censiti
        /// </returns>
        DeleteDocumento.DeleteDocumentoResponse DeleteDocumento(DeleteDocumento.DeleteDocumentoRequest request);

        #endregion

        #region Servizi per la gestione della profilazione

        /// <summary>
        /// Servizio per il reperimento dei profili
        /// </summary>
        /// <param name="request">
        /// Dati della richiesta
        /// </param>
        /// <returns>
        /// Il servizio potrà restituire i seguenti codici di errore se l'operazione non andrà a buon fine:
        /// - PARAMETRO_MANCANTE
        ///     Parametro obbligatorio non fornito
        ///     
        /// - TOKEN_NON_VALIDO
        ///     Il token di autenticazione fornito non è valido o scaduto
        ///     
        /// - ERRORE_NON_GESTITO
        ///     Si è verificato un errore non previsto tra quelli censiti        
        /// </returns>
        GetProfili.GetProfiliResponse GetProfili(GetProfili.GetProfiliRequest request);

        /// <summary>
        /// Servizio per il reperimento dei dati di un profilo
        /// </summary>
        /// <param name="request">
        /// Dati della richiesta
        /// </param>
        /// <returns>
        /// Il servizio potrà restituire i seguenti codici di errore se l'operazione non andrà a buon fine:
        /// - PARAMETRO_MANCANTE
        ///     Parametro obbligatorio non fornito
        ///     
        /// - TOKEN_NON_VALIDO
        ///     Il token di autenticazione fornito non è valido o scaduto
        ///     
        /// - ERRORE_NON_GESTITO
        ///     Si è verificato un errore non previsto tra quelli censiti        
        /// </returns>
        GetProfilo.GetProfiloResponse GetProfilo(GetProfilo.GetProfiloRequest request);

        #endregion

        #region Servizi per la gestione dei fascicoli

        /// <summary>
        /// Servizio di creazione di un fascicolo
        /// </summary>
        /// <param name="request">
        /// Dati della richiesta
        /// </param>
        /// <returns>
        /// Il servizio potrà restituire i seguenti codici di errore se l'operazione non andrà a buon fine:
        /// - PARAMETRO_MANCANTE
        ///     Parametro obbligatorio non fornito
        ///     
        /// - TOKEN_NON_VALIDO
        ///     Il token di autenticazione fornito non è valido o scaduto
        /// 
        /// </returns>
        CreateFascicolo.CreateFascicoloResponse CreateFascicolo(CreateFascicolo.CreateFascicoloRequest request);

        /// <summary>
        /// Servizio per la classificazione di un documento in un nodo di titolario
        /// </summary>
        /// <param name="request">
        /// Dati della richiesta
        /// </param>
        /// <returns>
        /// Il servizio potrà restituire i seguenti codici di errore se l'operazione non andrà a buon fine:
        /// - PARAMETRO_MANCANTE
        ///     Parametro obbligatorio non fornito
        ///     
        /// - TOKEN_NON_VALIDO
        ///     Il token di autenticazione fornito non è valido o scaduto
        /// 
        /// - NODO_TITOLARIO_NON_TROVATO
        ///     Nodo di titolario non trovato
        ///     
        /// - CLASSIFICAZIONE_FALLITA
        ///     Errore nella classificazione del documento
        ///     
        /// </returns>
        ClassificaDocumento.ClassificaDocumentoResponse ClassificaDocumento(ClassificaDocumento.ClassificaDocumentoRequest request);

        /// <summary>
        /// Servizio per la fascicolazione di un documento in un fascicolo procedimentale
        /// </summary>
        /// <param name="request">
        /// Dati della richiesta
        /// </param>
        /// <returns>
        /// Il servizio potrà restituire i seguenti codici di errore se l'operazione non andrà a buon fine:
        /// - PARAMETRO_MANCANTE
        ///     Parametro obbligatorio non fornito
        ///     
        /// - TOKEN_NON_VALIDO
        ///     Il token di autenticazione fornito non è valido o scaduto
        /// 
        /// - FASCICOLO_NON_TROVATO
        ///     Nodo di titolario non trovato
        ///     
        /// - FASCICOLAZIONE_FALLITA
        ///     Errore nella classificazione del documento
        ///     
        /// </returns>
        FascicolaDocumento.FascicolaDocumentoResponse FascicolaDocumento(FascicolaDocumento.FascicolaDocumentoRequest request);

        #endregion

        #region Servizi per l'invio a Centro Servizi e gestione dei supporti

        /// <summary>
        /// Servizio per l'invio del documento al Centro Servizi
        /// </summary>
        /// <param name="request">
        /// Dati della richiesta
        /// </param>
        /// Il servizio potrà restituire i seguenti codici di errore se l'operazione non andrà a buon fine:
        /// - PARAMETRO_MANCANTE
        ///     Parametro obbligatorio non fornito
        ///     
        /// - TOKEN_NON_VALIDO
        ///     Il token di autenticazione fornito non è valido o scaduto
        ///     
        /// 
        /// </returns>
        InviaCentroServizi.InviaCentroServiziResponse InviaCentroServizi(InviaCentroServizi.InviaCentroServiziRequest request);

        /// <summary>
        /// Servizio per il reperimento dei supporti di conservazione per un'istanza
        /// </summary>
        /// <param name="request">
        /// Dati della richiesta
        /// </param>
        /// Il servizio potrà restituire i seguenti codici di errore se l'operazione non andrà a buon fine:
        /// - PARAMETRO_MANCANTE
        ///     Parametro obbligatorio non fornito
        ///     
        /// - TOKEN_NON_VALIDO
        ///     Il token di autenticazione fornito non è valido o scaduto
        ///     
        /// 
        /// </returns>
        GetSupporti.GetSupportiResponse GetSupporti(GetSupporti.GetSupportiRequest request);

        #endregion
    }
}