using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VtDocs.BusinessServices.Entities.Authentication
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    //[System.Xml.Serialization.XmlType(Namespace = "http://www.valueteam.com/VtDocs/Business/Entities/Authentication/GetLoginRequest")]
    public class GetLoginRequest : Request
    {
        /// <summary>
        /// 
        /// </summary>
        public DocsPaVO.utente.UserLogin Login
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Forced
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string WebSessionId
        {
            get;
            set;
        }
    }
}
