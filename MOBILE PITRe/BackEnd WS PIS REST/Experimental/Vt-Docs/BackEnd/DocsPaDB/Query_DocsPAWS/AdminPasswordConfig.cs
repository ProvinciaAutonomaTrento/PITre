using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using DocsPaVO.amministrazione;
using log4net;

namespace DocsPaDB.Query_DocsPAWS
{
    /// <summary>
    /// Classe per la gestione delle password per l'amministrazione
    /// </summary>
    public class AdminPasswordConfig
    {
        private static ILog logger = LogManager.GetLogger(typeof(AdminPasswordConfig));
        public AdminPasswordConfig()
        {
        }

        #region Public methods

        /// <summary>
        /// Save dei dati per la gestione delle password in amministrazione
        /// </summary>
        /// <param name="configurations"></param>
        /// <returns></returns>
        public static bool SavePasswordConfigurations(PasswordConfigurations configurations)
        {
            bool saved = false;

            using (DBProvider dbProvider = new DBProvider())
            {
                dbProvider.BeginTransaction();

                // Reperimento configurazioni precedenti
                PasswordConfigurations oldConfigurations = GetPasswordConfigurations(configurations.IdAmministrazione);

                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("PASSWORD_CONFIG_SET_DATA");

                if (configurations.ExpirationEnabled)
                {
                    queryDef.setParam("enablePasswordExpiration", "1");
                    queryDef.setParam("passwordExpirationDays", configurations.ValidityDays.ToString());
                }
                else
                {
                    queryDef.setParam("enablePasswordExpiration", "NULL");
                    queryDef.setParam("passwordExpirationDays", "NULL");
                }
                
                if (configurations.MinLength > 0)
                    queryDef.setParam("passwordMinLength", configurations.MinLength.ToString());
                else
                    queryDef.setParam("passwordMinLength", "NULL");

                if (configurations.SpecialCharacters != null && configurations.SpecialCharacters.Length > 0)
                    queryDef.setParam("passwordSpecialChars", string.Format("'{0}'", new string(configurations.SpecialCharacters).Replace("'", "''")));
                else
                    queryDef.setParam("passwordSpecialChars", "NULL");

                if (configurations.IdAmministrazione == -1)
                    // System administrator
                    queryDef.setParam("idAmministrazione", " IS NULL");
                else
                    queryDef.setParam("idAmministrazione", " = " + configurations.IdAmministrazione.ToString());
                
                string commandText = queryDef.getSQL();
                //Debugger.Write(commandText);
                logger.Debug(commandText);
                int rowsAffected;
                if (!dbProvider.ExecuteNonQuery(commandText, out rowsAffected))
                    throw new ApplicationException(string.Format("Errore nell'aggiornamento delle configurazioni delle password. ID Amministrazione: {0}", configurations.IdAmministrazione.ToString()));
                
                saved = (rowsAffected == 1);

                // Se il numero di giorni di validità è cambiato
                // per ogni utente dell'amministrazione
                // viene aggiornata la data di creazione della password
                if (saved)
                {
                    SetCreationDateAllUser(dbProvider, configurations, oldConfigurations);
                }

                dbProvider.CommitTransaction();
            }

            return saved;
        }

        /// <summary>
        /// Imposta come scadute tutte le password per tutti gli utenti dell'amministrazione.
        /// Al prossimo login, tutti gli utenti saranno costretti a reimpostare la propria password.
        /// </summary>
        /// <param name="idAmministrazione"></param>
        public static void ExpireAllPassword(int idAmministrazione)
        {
            try
            {
                using (DBProvider dbProvider = new DBProvider())
                {
                    DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("PASSWORD_EXPIRATION_EXPIRE_ALL");

                    if (idAmministrazione == -1)
                        // System administrator
                        queryDef.setParam("idAmministrazione", " IS NULL");
                    else
                        queryDef.setParam("idAmministrazione", " = " + idAmministrazione.ToString());

                    string commandText = queryDef.getSQL();
                    //Debugger.Write(commandText);
                    logger.Debug(commandText);
                    if (!dbProvider.ExecuteNonQuery(commandText))
                        throw new ApplicationException(string.Format("Errore nell'impostazione della scadenza per tutte le password. ID Amministrazione: {0}", idAmministrazione.ToString()));
                }
            }
            catch (Exception ex)
            {
                //Debugger.Write(ex.Message);
                logger.Error(ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// Verifica se è abilitata l'autenticazione di dominio per l'amministrazione
        /// </summary>
        /// <param name="idAmministrazione"></param>
        /// <returns></returns>
        public static bool DomainAuthenticationEnabled(int idAmministrazione)
        {
            bool isEnabled = false;

            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("PASSWORD_DOMAIN_AUTH_ENABLED");

            if (idAmministrazione == -1)
                // System administrator
                queryDef.setParam("idAmministrazione", " IS NULL");
            else
                queryDef.setParam("idAmministrazione", " = " + idAmministrazione.ToString());

            string commandText = queryDef.getSQL();
            //Debugger.Write(commandText);
            logger.Debug(commandText);
            using (DBProvider dbProvider = new DBProvider())
            {
                string field;
                if (dbProvider.ExecuteScalar(out field, commandText))
                    isEnabled = !string.IsNullOrEmpty(field);
            }

            return isEnabled;
        }

        /// <summary>
        /// Reperimento dei dati per la gestione della password in amministrazione
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        internal static PasswordConfigurations GetPasswordConfigurations(string userId)
        {
            PasswordConfigurations config = null;

            using (DBProvider dbProvider = new DBProvider())
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("PASSWORD_CONFIG_GET_DATA_FROM_USER_ID");
                queryDef.setParam("userId", userId);
                
                string commandText = queryDef.getSQL();
                //Debugger.Write(commandText);
                logger.Debug(commandText);
                using (IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    if (reader.Read())
                    {
                        config = new PasswordConfigurations(reader);
                    }
                }
            }

            return config;
        }

        //MEV utenti multi-amministrazione
        /// <summary>
        /// Reperimento dei dati per la gestione della password in amministrazione
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        internal static PasswordConfigurations GetPasswordConfigurations(string userId, string codAmm)
        {
            PasswordConfigurations config = null;

            using (DBProvider dbProvider = new DBProvider())
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("PASSWORD_CONFIG_GET_DATA_FROM_USER_ID_BY_AMM");
                queryDef.setParam("userId", userId);
                queryDef.setParam("codAmm", codAmm);

                string commandText = queryDef.getSQL();
                logger.Debug(commandText);
                using (IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    if (reader.Read())
                    {
                        config = new PasswordConfigurations(reader);
                    }
                }
            }

            return config;
        }




        /// <summary>
        /// Reperimento dei dati per la gestione della password in amministrazione
        /// </summary>
        /// <returns></returns>
        public static PasswordConfigurations GetPasswordConfigurations(int idAmministrazione)
        {
            PasswordConfigurations config = null;

            if (idAmministrazione > 0)
            {
                using (DBProvider dbProvider = new DBProvider())
                {
                    DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("PASSWORD_CONFIG_GET_DATA");
                    queryDef.setParam("idAmministrazione", " = " + idAmministrazione.ToString());

                    string commandText = queryDef.getSQL();
                    //Debugger.Write(commandText);
                    logger.Debug(commandText);
                    using (IDataReader reader = dbProvider.ExecuteReader(commandText))
                    {
                        if (reader.Read())
                        {
                            config = new PasswordConfigurations(reader);
                        }
                    }
                }
            }

            return config;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Impostazione della data di creazione della password 
        /// per tutti gli utenti dell'amministrazione.
        /// <remarks>
        /// Regole per l'impostazione:
        ///     - l'aggiornamento viene effettuato solamente se il valore
        ///       per i giorni di validità è stato modificato
        ///     - se il numero di giorni impostato è 0, la scadenza
        ///       password è stata disabilitata, quindi per tutti gli utenti
        ///       verrà rimossa la data di creazione
        ///     - se il numero di giorni precedente a quello attuale era 0,
        ///       verrà aggiornata per tutti gli utenti la data di creazione
        /// </remarks>
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="configurations"></param>
        /// <param name="oldConfigurations"></param>
        /// <returns></returns>
        private static void SetCreationDateAllUser(DBProvider dbProvider, PasswordConfigurations configurations, PasswordConfigurations oldConfigurations)
        {
            if (oldConfigurations.ValidityDays != configurations.ValidityDays)
            {
                if (configurations.ValidityDays == 0)
                {
                    SetCreationDateAllUser(dbProvider, configurations.IdAmministrazione, false);
                }
                else if (oldConfigurations.ValidityDays == 0)
                {
                    SetCreationDateAllUser(dbProvider, configurations.IdAmministrazione, true);
                }
            }
        }

        /// <summary>
        /// Impostazione della data di creazione della password
        /// </summary>
        /// <param name="dbProvider"></param>
        /// <param name="idAmministrazione"></param>
        /// <param name="udpateDate"></param>
        private static void SetCreationDateAllUser(DBProvider dbProvider, int idAmministrazione, bool udpateDate)
        {
            // Impostazione della data di creazione della password 
            // per tutti gli utenti dell'amministrazione 
            // (questo per non permetterne la scadenza immediata)
            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("PASSWORD_EXPIRATION_SET_CREATION_DATE_ALL_USER");

            if (udpateDate)
                queryDef.setParam("passwordCreationDate", DocsPaDbManagement.Functions.Functions.ToDate(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")));
            else
                // Se la scadenza password viene disabilitata, la data di creazione delle password viene rimossa
                queryDef.setParam("passwordCreationDate", "NULL");

            if (idAmministrazione == -1)
                // System administrator
                queryDef.setParam("idAmministrazione", " IS NULL");
            else
                queryDef.setParam("idAmministrazione", " = " + idAmministrazione.ToString());

            string commandText = queryDef.getSQL();
            //Debugger.Write(commandText);
            logger.Debug(commandText);
            if (!dbProvider.ExecuteNonQuery(commandText))
                throw new ApplicationException(string.Format("Errore nell'impostazione della data di creazione delle password per tutti gli utenti. ID Amministrazione: {0}", idAmministrazione.ToString()));
        }

        #endregion
    }
}
