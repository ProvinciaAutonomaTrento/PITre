using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using Dpa.DataAccess;

namespace Rubrica.Library
{
    /// <summary>
    /// 
    /// </summary>
    internal sealed class RubricaDatabase
    {
        public const string SQL_PROVIDER = "Sql";
        public const string ORACLE_PROVIDER = "Oracle";
        public const string ORACLECLIENT_PROVIDER = "OracleClient";

        /// <summary>
        /// Reperimento stringa di connessione per l'accesso al database RubricaComune
        /// </summary>
        /// <returns></returns>
        public static string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings[Properties.Resources.RubricaDatabaseName].ConnectionString;
        }

        /// <summary>
        /// Reperimento del nome corretto per la stored procedure relativamente al motore di database utilizzato
        /// </summary>
        /// <param name="spName"></param>
        /// <returns></returns>
        public static string GetSpNameForPackage(string spName)
        {
            string providerName = ConfigurationManager.ConnectionStrings[Properties.Resources.RubricaDatabaseName].ProviderName;

            if (providerName == ORACLE_PROVIDER || providerName == ORACLECLIENT_PROVIDER)
                return string.Format("R.{0}", spName);
            else if (providerName == SQL_PROVIDER)
                return spName;
            else
                throw new ApplicationException(string.Format(Properties.Resources.ProviderNotSupportedException, providerName));
        }

        /// <summary>
        /// Creazione oggetto Database per l'accesso al db rubrica comune
        /// </summary>
        /// <returns></returns>
        public static Database CreateDatabase()
        {
            string providerName = ConfigurationManager.ConnectionStrings[Properties.Resources.RubricaDatabaseName].ProviderName;

            if (providerName == SQL_PROVIDER)
                return new Dpa.DataAccess.Sql.SqlDatabase(GetConnectionString());
            else if (providerName == ORACLE_PROVIDER)
                return new Dpa.DataAccess.Oracle.OracleDatabase(GetConnectionString());
            else if (providerName == ORACLECLIENT_PROVIDER)
                return new Dpa.DataAccess.OracleClient.OracleDatabase(GetConnectionString());
            else
                throw new ApplicationException(string.Format(Properties.Resources.ProviderNotSupportedException, providerName));
        }
    }
}