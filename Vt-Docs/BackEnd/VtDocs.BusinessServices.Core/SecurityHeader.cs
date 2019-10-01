using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services.Protocols;

namespace VtDocs.BusinessServices.Entities
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class SecurityHeader : SoapHeader
    {
        /// <summary>
        /// 
        /// </summary>
        public DataObject InfoUtente
        {
            get;
            set;
        }
    }
}
