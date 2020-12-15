using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.Validations;

namespace DocsPaVO.utente.Repertori.RequestAndResponse
{
    /// <summary>
    /// Response del servizio di salvataggio modifiche apportate ad un registro di reprtorio
    /// </summary>
    [Serializable()]
    public class SaveRegistroRepertorioSettingsResponse
    {
        /// <summary>
        /// Esito della validazione dei dati da salvare
        /// </summary>
        public ValidationResultInfo ValidationResult { get; set; }

        /// <summary>
        /// Risultato dell'operazione di salvataggio delle modifiche. 
        /// Se è negativa, consultare ValidationResult per informazioni sulle eventuali 
        /// validazioni fallite
        /// </summary>
        public bool SaveChangesResult { get; set; }
    }
}
