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
    public class GetSchedaUtenteResponse : Response
    {
        ///// <summary>
        ///// Indica l'identificativo univoco dell'utente per cui è stata effettuata la richiesta
        ///// </summary>
        //public string IdUtente
        //{
        //    get;
        //    set;
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        //public string UserId
        //{
        //    get;
        //    set;
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        //public string MatricolaUtente
        //{
        //    get;
        //    set;
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        //public string DescrizioneUtente
        //{
        //    get;
        //    set;
        //}

        /// <summary>
        /// 
        /// </summary>
        public DocsPaVO.utente.Utente Utente
        {
            get;
            set;
        }

        /// <summary>
        /// Lista delle unità organizzative di appartenenza dell'utente
        /// </summary>
        public UO[] UnitaOrganizzative
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public List<DocsPaVO.Qualifica.PeopleGroupsQualifiche> Qualifiche
        {
            get;
            set;
        }
    }
}
