using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VtDocs.BusinessServices.Entities.Administration.Organigramma
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class GetSchedaUtenteRequest : Request
    {
        /// <summary>
        /// Indica l'identificativo univico dell'utente per cui determinare la situazione in organigramma
        /// </summary>
        /// <remarks>
        /// Se non valorizzato, si utilizzerà l'id dell'utente richiedente del servizio
        /// </remarks>
        public string IdUtente
        {
            get;
            set;
        }

        /// <summary>
        /// Indica se restituire o meno le qualifiche assegnate all'utente indipendentemente
        /// </summary>
        public bool GetQualifiche
        {
            get;
            set;
        }

        /// <summary>
        /// Indica se far restituire al servizio solamente il primo superiore identificato per ciascun ruolo dell'utente richiesto
        /// </summary>
        public bool GetPrimoSuperiorePerRuolo
        {
            get;
            set;
        }

        /// <summary>
        /// Indica se reperire gli utenti / ruoli superiori indipendentemente dal livello del tipo ruolo
        /// </summary>
        public bool IgnoraLivelloSuperiori
        {
            get;
            set;
        }
    }
}
