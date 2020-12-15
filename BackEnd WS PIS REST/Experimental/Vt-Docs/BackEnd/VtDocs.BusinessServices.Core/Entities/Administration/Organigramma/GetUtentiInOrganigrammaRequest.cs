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
    public class GetUtentiInOrganigrammaRequest : Request
    {
        /// <summary>
        /// 
        /// </summary>
        public GetUtentiInOrganigrammaRequest()
        {
            this.Recursive = true;
        }

        /// <summary>
        /// Id dell'unità organizzativa a partire da cui reperire gli utenti
        /// </summary>
        public string IdUo
        {
            get;
            set;
        }

        /// <summary>
        /// Indica il filtro per denominazione utente
        /// </summary>
        public string FiltroPerDenominazione
        {
            get;
            set;
        }

        /// <summary>
        /// Indica il filtro per codice qualifica per estrarre 
        /// solo gli utenti in organigramma che hanno una determinata qualifica
        /// </summary>
        public string FiltroPerCodiceQualifica
        {
            get;
            set;
        }

        /// <summary>
        /// Indica se estrarre nella ricerca solamente gli utenti censiti in un ruolo responsabile
        /// </summary>
        public bool FiltroPerUtentiResponsabili
        {
            get;
            set;
        }

        /// <summary>
        /// Indica se ricercare gli utenti ricorsivamente alle uo figlie
        /// </summary>
        public bool Recursive
        {
            get;
            set;
        }

        /// <summary>
        /// Qualifica che consente di discriminare il reperimento delle uo figlie
        /// </summary>
        public string CodiceQualificaDiscriminanteIfRecursive
        {
            get;
            set;
        }
    }
}
