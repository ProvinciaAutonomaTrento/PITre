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
    public class CredentialsHeader : SoapHeader
    {
        /// <summary>
        /// 
        /// </summary>
        public string UserName
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Password
        {
            get;
            set;
        }
    }
}
