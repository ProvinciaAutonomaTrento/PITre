using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VtDocs.BusinessServices.Entities.Document
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class GetVisibilitaResponse : Response
    {
        /// <summary>
        /// 
        /// </summary>
        public DocsPaVO.documento.DirittoOggetto[] Diritti
        {
            get;
            set;
        }
    }
}
