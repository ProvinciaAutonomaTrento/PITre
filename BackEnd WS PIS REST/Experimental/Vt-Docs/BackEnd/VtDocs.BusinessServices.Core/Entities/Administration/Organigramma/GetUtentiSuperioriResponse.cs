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
    public class GetUtentiSuperioriResponse : Response
    {
        /// <summary>
        /// 
        /// </summary>
        public GetUtentiSuperioriResponse()
        {
            this.Superiori = new List<UtenteInRuolo>();
            this.RuoliSuperiori = new List<RuoloInUO>();
        }

        /// <summary>
        /// 
        /// </summary>
        public List<UtenteInRuolo> Superiori
        {
            get;
            set;
        }


        /// <summary>
        /// 
        /// </summary>
        public List<RuoloInUO> RuoliSuperiori
        {
            get;
            set;
        }
    }
}
