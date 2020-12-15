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
    public class OracleConnectionStringSettings : ConnectionStringSettings
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "ORACLE";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string GetProviderName()
        {
            return "Oracle";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string GetConnectionString()
        {
            return string.Format("server={0}; User Id={1}; Password={2};", 
                    this.ServerName, this.UserName, this.Password);
        }
    }
}
