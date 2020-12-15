using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaWS.Conservazione.PacchettiVersamento
{
    /// <summary>
    /// Classe che censisce tutti i possibili codici di errore riscontrabili nei servizi di versamento
    /// </summary>
    public sealed class ErrorCodes
    {
        /// <summary>
        /// Codice di errore non gestito
        /// </summary>
        public const string ERRORE_NON_GESTITO = "ERRORE_NON_GESTITO";

        /// <summary>
        /// Codice di errore relativo ad un parametro in ingresso non fornito
        /// </summary>
        public const string PARAMETRO_MANCANTE = "PARAMETRO_MANCANTE";

        /// <summary>
        /// Codice di errore relativo all'autenticazione dell'utente fallita 
        /// </summary>
        public const string AUTENTICAZIONE_FALLITA = "AUTENTICAZIONE_FALLITA";

        /// <summary>
        /// Codice di errore relativo al fatto che l'utente non è abilitato all'utilizzo dei servizi
        /// </summary>
        public const string UTENTE_NON_ABILITATO = "UTENTE_NON_ABILITATO";

        /// <summary>
        /// Codice di errore relativo al passaggio di un token di autenticazione non valido o scaduto
        /// </summary>
        public const string TOKEN_NON_VALIDO = "TOKEN_NON_VALIDO";

        /// <summary>
        /// Codice di errore per istanza di conservazione non cancellabile
        /// </summary>
        public const string ISTANZA_NON_CANCELLABILE = "ISTANZA_NON_CANCELLABILE";

        /// <summary>
        /// Codice di errore per istanza di conservazione non trovata
        /// </summary>
        public const string ISTANZA_NON_TROVATA = "ISTANZA_NON_TROVATA";

        /// <summary>
        /// Codice di errore in fase di upload del file in conservazione
        /// </summary>
        public const string UPLOAD_FILE_FALLITO = "UPLOAD_FILE_FALLITO";

        /// <summary>
        /// Codice di errore relativo al controllo sul contenuto del file rispetto al formato
        /// </summary>
        public const string CONTENUTO_FILE_NON_CONFORME = "CONTENUTO_FILE_NON_CONFORME";

        /// <summary>
        /// Codice di errore in fase di download del file di un documento
        /// </summary>
        public const string DOWNLOAD_FILE_FALLITO = "DOWNLOAD_FILE_FALLITO";

        /// <summary>
        /// Codice di errore in fase di creazione di un nuovo documento
        /// </summary>
        public const string CREAZIONE_DOCUMENTO_FALLITA = "CREAZIONE_DOCUMENTO_FALLITA";

        /// <summary>
        /// Codice di errore relativo al documento inesistente
        /// </summary>
        public const string DOCUMENTO_NON_TROVATO = "DOCUMENTO_NON_TROVATO";

        /// <summary>
        /// Codice di errore relativo al documento non trovato nell'istanza di conservazione
        /// </summary>
        public const string DOCUMENTO_NON_TROVATO_IN_ISTANZA = "DOCUMENTO_NON_TROVATO_IN_ISTANZA";

        /// <summary>
        /// Codice di errore relativo ad un documento non cancellabile in un'istanza di conservazione
        /// </summary>
        public const string DOCUMENTO_NON_CANCELLABILE = "DOCUMENTO_NON_CANCELLABILE";

        /// <summary>
        /// Codice di errore relativo all'inserimento fallito di un documento in istanza di conservazione
        /// </summary>
        public const string INSERIMENTO_IN_ISTANZA_FALLITO = "INSERIMENTO_IN_ISTANZA_FALLITO";

        /// <summary>
        /// Codice di errore relativo al profilo documentale non trovato
        /// </summary>
        public const string PROFILO_NON_TROVATO = "PROFILO_NON_TROVATO";

        /// <summary>
        /// Codice di errore relativo ad un campo del profilo documentale non trovato
        /// </summary>
        public const string CAMPO_PROFILO_NON_TROVATO = "CAMPO_PROFILO_NON_TROVATO";

        /// <summary>
        /// Codice di errore relativo ad un campo del profilo documentale obbligatorio
        /// </summary>
        public const string VALORE_CAMPO_PROFILO_OBBLIGATORIO = "VALORE_CAMPO_PROFILO_OBBLIGATORIO";

        /// <summary>
        /// Codice di errore relativo ad un nodo di titolario non trovato
        /// </summary>
        public const string NODO_TITOLARIO_NON_TROVATO = "NODO_TITOLARIO_NON_TROVATO";

        /// <summary>
        /// Codice di errore relativo alla creazione del fascicolo
        /// </summary>
        public const string CREAZIONE_FASCICOLO_FALLITA = "CREAZIONE_FASCICOLO_FALLITA";

        /// <summary>
        /// Codice di errore relativo alla creazione del sottofascicolo
        /// </summary>
        public const string CREAZIONE_SOTTOFASCICOLO_FALLITA = "CREAZIONE_SOTTOFASCICOLO_FALLITA";

        /// <summary>
        /// Codice di errore relativo alla fascicolazione di un documento
        /// </summary>
        public const string FASCICOLAZIONE_FALLITA = "FASCICOLAZIONE_FALLITA";

        /// <summary>
        /// Codice di errore relativo al fascicolo non trovato
        /// </summary>
        public const string FASCICOLO_NON_TROVATO = "FASCICOLO_NON_TROVATO";

        /// <summary>
        /// Codice di errore relativo alla classificazione di un documento
        /// </summary>
        public const string CLASSIFICAZIONE_FALLITA = "CLASSIFICAZIONE_FALLITA";

        /// <summary>
        /// Codice di errore relativo all'invio a Centro Servizi fallito
        /// </summary>
        public const string INVIO_CENTRO_SERVIZI_FALLITO = "INVIO_CENTRO_SERVIZI_FALLITO";

        /// <summary>
        /// Codice di errore relativo ai supporti non trovati nell'istanza di conservazione
        /// </summary>
        public const string SUPPORTI_NON_TROVATI = "SUPPORTI_NON_TROVATI";
    }
}