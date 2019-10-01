using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VtDocs.BusinessServices.Entities
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class GetConfigurazioneResponse : Response
    {
        /// <summary>
        /// 
        /// </summary>
        public DocsPaVO.amministrazione.ChiaveConfigurazione ChiaveConfigurazione
        {
            get;
            set;
        }
    }
}
