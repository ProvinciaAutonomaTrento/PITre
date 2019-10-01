using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rubrica.ConfigCustomAction
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public abstract class ConnectionStringSettings
    {
        /// <summary>
        /// 
        /// </summary>
        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.Description("Server")]
        public string ServerName
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.Description("Nome utente")]
        public string UserName
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.Description("Password utente")]
        public string Password
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract string GetProviderName();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract string GetConnectionString();
    }
}
