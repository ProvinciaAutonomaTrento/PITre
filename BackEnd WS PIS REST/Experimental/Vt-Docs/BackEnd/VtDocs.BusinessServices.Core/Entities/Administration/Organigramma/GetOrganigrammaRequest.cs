using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VtDocs.BusinessServices.Entities.Administration.Organigramma
{
    /// <summary>
    /// Oggetto request relativo al servizio di reperimento delle unità organizzative
    /// </summary>
    [Serializable()]
    public class GetOrganigrammaRequest : Request
    {
        /// <summary>
        /// 
        /// </summary>
        public GetOrganigrammaRequest()
        {
            this.IdUnitaOrganizzativa = 0;
            //this.Livello = "0";
        }

        /// <summary>
        /// Id dell'amministrazione di appartenenza dell'organigramma
        /// </summary>
        public string IdAmministrazione
        {
            get;
            set;
        }

        /// <summary>
        /// Identificativo univoco dell'unità organizzativa parent
        /// </summary>
        public int IdUnitaOrganizzativa
        {
            get;
            set;
        }

        ///// <summary>
        ///// Livello di annidamento gerarchico
        ///// </summary>
        //public string Livello
        //{
        //    get;
        //    set;
        //}

        /// <summary>
        /// Indica se caricare ricorsivamente tutta la struttura dell'organigramma a partire dall'identificativo dell'unità organizzativa fornito
        /// </summary>
        public bool Recursive
        {
            get;
            set;
        }

        /// <summary>
        /// Indica, solo se il caricamento non è ricorsivo, se caricare o meno i ruoli dell'unità organizzativa
        /// </summary>
        public bool FetchRuoliIfNotRecursive
        {
            get;
            set;
        }

        /// <summary>
        /// Indica se restituire o meno le qualifiche degli utenti nei ruoli dell'organigramma
        /// </summary>
        public bool GetQualificheUtentiInRuolo
        {
            get;
            set;
        }
    }
}