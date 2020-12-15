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
    public class RemoveAssociazioneUtenteRuoloResponse : Response
    {
        /// <summary>
        /// 
        /// </summary>
        public string IdRuolo
        {
            get;
            set;
        }
    }
}
