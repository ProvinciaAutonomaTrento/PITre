using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rubrica.Library.Security
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class SecurityCredentialsResult
    {
        /// <summary>
        /// 
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool Amministratore { get; set; }
    }
}
