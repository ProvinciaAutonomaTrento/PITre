// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Serilog;
using System;

namespace DocsPaUtils.Interfaces.DbManagement
{
    /// <summary>
    /// Class factory per la creazione delle istanze concrete
    /// dei gestori database
    /// </summary>
    public sealed class DatabaseFactory
    {
        private static ILogger logger = Log.ForContext(typeof(DatabaseFactory));
        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public static IDatabase CreateDatabase()
        {
            // Reperimento tipo concreto del database corrente
            Type concreteType = GetDatabaseType();

            if (concreteType != null)
            {
                return (IDatabase) Activator.CreateInstance(concreteType);
            }
            else
            {
                // Non è stato possibile creare l'istanza concreta del database,
                // effetuare log di "scorta" utilizzando la chiave esistente su file web.config "SYSTEM_LOG_PATH"
                logger.Debug(string.Format("Database non valido"));

                return null;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public static bool IsValidDatabase()
        {
            return (GetDatabaseType() != null);
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        private static Type GetDatabaseType()
        {
            const string SUFFIX = "_DATABASE_TYPE";

            string configName = string.Concat(GetCurrentDbType(), SUFFIX);
            string concreteTypeName = DocsPaUtils.Properties.Resources.ResourceManager.GetString(configName);

            return Type.GetType(concreteTypeName, false);
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        private static string GetCurrentDbType()
        {
            return DocsPaVO.Settings.AppSettings.Instance.DBType.ToUpper();
        }
    }
}
