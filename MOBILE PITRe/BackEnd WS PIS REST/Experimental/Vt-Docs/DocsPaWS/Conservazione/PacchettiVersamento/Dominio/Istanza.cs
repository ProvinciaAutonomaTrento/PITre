using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaWS.Conservazione.PacchettiVersamento.Dominio
{
    /// <summary>
    /// Oggetto dati contenente i dettagli dell'istanza di conservazione
    /// </summary>
    [Serializable()]
    [System.Xml.Serialization.XmlType(Namespace = "http://www.valueteam.com/Conservazione/PacchettiVersamento/Istanza")]
    public class Istanza
    {
        /// <summary>
        /// Identificativo univoco dell'istanza di conservazione
        /// </summary>
        /// <remarks>
        /// Il dato è contatore autoincrement restituito dal sistema 
        /// </remarks>
        public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// Utente creatore dell'istanza di conservazione
        /// </summary>
        public string Richiedente
        {
            get;
            set;
        }

        /// <summary>
        /// Descrizione dell'istanza di conservazione
        /// </summary>
        /// <remarks>
        /// Dato obbligatorio
        /// </remarks>
        public string Descrizione
        {
            get;
            set;
        }

        /// <summary>
        /// Note di invio dell'istanza di conservazione
        /// </summary>
        /// <remarks>
        /// Dato facoltativo
        /// </remarks>
        public string NoteDiInvio
        {
            get;
            set;
        }

        /// <summary>
        /// Tipologia dell'istanza di conservazione
        /// </summary>
        /// <remarks>
        /// Valore predefinito: "Conservazione"
        /// </remarks>
        public TipiIstanzaEnum Tipo
        {
            get;
            set;
        }

        /// <summary>
        /// Indica se istruire il sistema nel consolidare i documenti nel momento in cui l'istanza sarà inviata a centro servizi
        /// </summary>
        public bool ConsolidaDocumentiSeInterna
        {
            get;
            set;
        }

        /// <summary>
        /// Stato cui si trova l'istanza di conservazione
        /// </summary>
        /// <remarks>
        /// Valore predefinito: "DaInviare"
        /// </remarks>
        public StatiIstanzaEnum Stato
        {
            get;
            set;
        }

        /// <summary>
        /// Data di apertura dell'istanza di conservazione
        /// </summary>
        /// <remarks>
        /// Il dato è sempre restituito dal sistema
        /// </remarks>
        public string DataApertura
        {
            get;
            set;
        }

        /// <summary>
        /// Data in cui l'istanza di conservazione è stata inviata al Centro Servizi
        /// </summary>
        /// <remarks>
        /// A seguito dell'invio dell'istanza, l'esecuzione di alcuni servizi di modifica per i Pacchetti di Versamento potranno non essere consentiti alle applicaizioni esterne
        /// </remarks>
        public string DataInvio
        {
            get;
            set;
        }

        /// <summary>
        /// Data di chiusura dell'istanza di conservazione
        /// </summary>
        /// <remarks>
        /// A seguito della chiusura dell'istanza, l'esecuzione di alcuni servizi di modifica per i Pacchetti di Versamento potranno non essere consentiti alle applicaizioni esterne
        /// </remarks>
        public string DataChiusura
        {
            get;
            set;
        }
    }
}