using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rubrica.ConfigCustomAction
{
    /// <summary>
    /// 
    /// </summary>
    public class SqlConnectionStringSettings : ConnectionStringSettings
    {
        /// <summary>
        /// 
        /// </summary>
        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.Description("Nome database")]
        public string Database
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "SQL SERVER";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string GetProviderName()
        {
            return "Sql";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string GetConnectionString()
        {
            return string.Format("server={0}; uid={1}; pwd={2}; database={3};",
                this.ServerName, this.UserName, this.Password, this.Database);
        }
    }
}
