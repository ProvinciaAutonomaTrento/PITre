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
    //[System.Xml.Serialization.XmlType(Namespace = "http://www.valueteam.com/VtDocs/Business/Entities/Authentication/GetLoginResponse")]
    public class GetLoginResponse : Response
    { 
        /// <summary>
        /// 
        /// </summary>
        public DocsPaVO.utente.UserLogin.LoginResult Result
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string IpAddress
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public DocsPaVO.utente.Utente User
        {
            get;
            set;
        }
    }
}
