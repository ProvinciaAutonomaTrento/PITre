using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Subscriber.DataAccess
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class DbHelper
    {
        /// <summary>
        /// 
        /// </summary>
        public const string DB_KEY = "STORAGE";

        /// <summary>
        /// 
        /// </summary>
        public const string SQL_PROVIDER = "System.Data.Sql";
        
        /// <summary>
        /// 
        /// </summary>
        public const string ORACLECLIENT_PROVIDER = "System.Data.OracleClient";

        /// <summary>
        /// Reperimento stringa di connessione per l'accesso al database RubricaComune
        /// </summary>
        /// <returns></returns>
        public static string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings[DB_KEY].ConnectionString;
        }

        /// <summary>
        /// Reperimento del nome corretto per la stored procedure relativamente al motore di database utilizzato
        /// </summary>
        /// <param name="spName"></param>
        /// <returns></returns>
        public static string GetSpNameForPackage(string spName)
        {
            string providerName = ConfigurationManager.ConnectionStrings[DB_KEY].ProviderName;
            string packageName = ConfigurationManager.AppSettings["ORACLE_PACKAGE"];

            if (providerName == ORACLECLIENT_PROVIDER)
                return string.Format("{0}.{1}", packageName, spName);
            else if (providerName == SQL_PROVIDER)
                return spName;
            else
                //throw new ApplicationException(string.Format(Properties.Resources.ProviderNotSupportedException, providerName));
                throw new ApplicationException();
        }

        /// <summary>
        /// Creazione oggetto Database per l'accesso al db rubrica comune
        /// </summary>
        /// <returns></returns>
        public static Database CreateDatabase()
        {
            string providerName = ConfigurationManager.ConnectionStrings[DB_KEY].ProviderName;

            if (providerName == SQL_PROVIDER)
                return new DataAccess.Sql.SqlDatabase(GetConnectionString());
            else if (providerName == ORACLECLIENT_PROVIDER)
                return new DataAccess.Oracle.OracleDatabase(GetConnectionString());
            else
                return null;

            //    throw new ApplicationException(string.Format(Properties.Resources.ProviderNotSupportedException, providerName));
        }
    }
}
