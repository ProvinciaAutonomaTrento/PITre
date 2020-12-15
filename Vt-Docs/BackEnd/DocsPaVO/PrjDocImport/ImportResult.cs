using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.PrjDocImport
{
    /// <summary>
    /// Oggetto utilizzato per fornire informazioni all'utente relativamente
    /// all'esito dell'importazione di un fascicolo o di un documento.
    /// </summary>
    [Serializable()]
    public class ImportResult
    {
        // Il risultato può essere Successo, Insuccesso e Problemi
        public enum OutcomeEnumeration
        {
            OK = 1,
            KO = 0,
            Warnings = 2,
            NONE =3,
            FileNotAcquired=4
        };

        /// <summary>
        /// Creazione di un nuovo oggetto result
        /// </summary>
        public ImportResult()
        {
            // Viene creata la lista delle informazioni
            this.OtherInformation = new List<string>();
        }

        /// <summary>
        /// Il risultato dell'importazione
        /// </summary>
        public OutcomeEnumeration Outcome { get; set; }

        /// <summary>
        /// Un messaggio con informazioni di dettaglio da mostrare all'utente
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Una lista di stringhe utilizzata per fornire all'utente ulteriori
        /// informazioni sull'esito
        /// </summary>
        public List<String> OtherInformation { get; set; }

        /// <summary>
        /// Stringa utilizzata per identificare la riga a cui si riferisce 
        /// questo oggetto result.
        /// </summary>
        public string Ordinal { get; set; }

        /// <summary>
        /// L'identificativo del documento
        /// </summary>
        public string DocNumber { get; set; }

        /// <summary>
        /// L'identificativo univoco del documento (system id)
        /// </summary>
        public string IdProfile { get; set; }

    }
}