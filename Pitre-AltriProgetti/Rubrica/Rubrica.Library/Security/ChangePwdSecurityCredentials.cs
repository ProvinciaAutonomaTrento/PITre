using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace Rubrica.Library.Security
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class ChangePwdSecurityCredentials : SecurityCredentials
    {
        /// <summary>
        /// 
        /// </summary>
        public ChangePwdSecurityCredentials()
        { }
        
        /// <summary>
        /// 
        /// </summary>
        public string NewPassword { get; set; }
    }
}