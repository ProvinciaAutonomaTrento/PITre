using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VtDocs.BusinessServices.Entities.Administration.Organigramma
{
    /// <summary>
    /// Dati dell'Unità Organizzativa estratti dal servizio GetListUOByQualificheUtente
    /// </summary>
    [Serializable()]
    public class UOByQualifica
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Codice
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Descrizione
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Classifica
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int Livello
        {
            get;
            set;
        }
    }
}
