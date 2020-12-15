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
    public class GetUtentiSuperioriRequest : Request
    {
        /// <summary>
        /// Identificativo univoco dell'utente per cui reperire i responsabili
        /// </summary>
        public string IdUtente
        {
            get;
            set;
        }

        /// <summary>
        /// Indica l'eventuale qualifica richiesta per il responsabile
        /// </summary>
        public string CodiceQualifica
        {
            get;
            set;
        }

        /// <summary>
        /// Indica se far restituire al servizio solamente il primo superiore identificato
        /// </summary>
        public bool GetPrimoSuperiore
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

        /// <summary>
        /// Indica se restituire solo i superiori del ruolo corrente.
        /// Se false, restituisce tutti i superiori di tutti i ruoli di cui fa parte l'utente.
        /// </summary>
        public bool GetSuperioriRuoloCorrente
        {
            get;
            set;
        }

        public string IdUO
        {
            get;
            set;
        }
    }
}
