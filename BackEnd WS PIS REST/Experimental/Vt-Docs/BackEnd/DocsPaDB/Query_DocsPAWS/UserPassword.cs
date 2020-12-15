using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using log4net;

namespace DocsPaDB.Query_DocsPAWS
{
    /// <summary>
    /// 
    /// </summary>
    public class PasswordExpirationNotEnabledException : ApplicationException
    {
        public PasswordExpirationNotEnabledException()
            : base()
        {
        }

        public PasswordExpirationNotEnabledException(string message)
            : base(message)
        {
        }

        public PasswordExpirationNotEnabledException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Classe di accesso ai dati per gestire i servizi di validazione della password dell'utente
    /// </summary>
    public class UserPassword
    {
        private ILog logger=LogManager.GetLogger(typeof(UserPassword));
        private string _userId = string.Empty;
        private string _idAmministrazione = string.Empty;

        /// <summary>
        /// Se true, indica che a livello di amministrazione
        /// è attiva la gestione della validità delle password
        /// </summary>
        // private bool _adminPasswordExpirationEnabled = false;

        private DocsPaVO.amministrazione.PasswordConfigurations _passwordConfigurations = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="utente"></param>
        public UserPassword(string userId) //, int idAmministrazione)
        {
            this._userId = userId;
//            this._idAmministrazione = idAmministrazione;
        }

        public UserPassword(string userId, string idAmministrazione)
        {
            this._userId = userId;
            this._idAmministrazione = idAmministrazione;
        }

        #region Public methods

        /// <summary>
        /// Modifica delle credenziali utente
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="idAmministrazione"></param>
        public void ChangeUser(string userId) // , int idAmministrazione)
        {
            this._userId = userId;
            //this._idAmministrazione = idAmministrazione;
        }

        /// <summary>
        /// Verifica se la password per l'utente è scaduta
        /// </summary>
        /// <returns></returns>
        public bool IsPasswordExpired()
        {
            bool expired = false;

            UserPasswordData data = this.GetPasswordData();

            if (data.UserType != UserPasswordData.UserTypesEnum.User)
            {
                // La password è senza scadenza qualora l'utente sia un amministratore (di qualsiasi tipo)
                expired = false;
            }
            else if (data.CreationDate == DateTime.MinValue)
            {
                // Se la data di creazione della password non è impostata,
                // la password è considerata scaduta (anche se è stata definita "Nessuna scadenza password")
                expired = true;
            }
            else if (!data.PasswordNeverExpire)
            {
                DateTime actualDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                expired = (data.CreationDate > actualDate);

                if (!expired)
                {
                    DateTime expireDate = data.CreationDate.AddDays((double)this.GetAdminPasswordConfig().ValidityDays);
                    expired = (actualDate >= expireDate);
                }
            }

            return expired;
        }

        // //MEV utenti multi-amministrazione
        /// <summary>
        /// Verifica se la password per l'utente di un'amministrazione è scaduta
        /// </summary>
        /// <returns></returns>
        public bool IsPasswordUserInAmmExpired()
        {
            bool expired = false;

            UserPasswordData data = this.GetPassworUserInAmmdData();

            if (data.UserType != UserPasswordData.UserTypesEnum.User)
            {
                // La password è senza scadenza qualora l'utente sia un amministratore (di qualsiasi tipo)
                expired = false;
            }
            else if (data.CreationDate == DateTime.MinValue)
            {
                // Se la data di creazione della password non è impostata,
                // la password è considerata scaduta (anche se è stata definita "Nessuna scadenza password")
                expired = true;
            }
            else if (!data.PasswordNeverExpire)
            {
                DateTime actualDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                expired = (data.CreationDate > actualDate);

                if (!expired)
                {
                    DateTime expireDate = data.CreationDate.AddDays((double)this.GetUserAdminPasswordConfig().ValidityDays);
                    expired = (actualDate >= expireDate);
                }
            }

            return expired;
        }




        /// <summary>
        /// Calcolo del giorno di scadenza della password per l'utente
        /// </summary>
        /// <returns></returns>
        public DateTime GetPasswordExpirationDate()
        {
            UserPasswordData data = this.GetPasswordData();

            if (data.PasswordNeverExpire)
            {
                // Giorno di scadenza illimitato
                return DateTime.MaxValue;
            }
            else if (data.CreationDate == DateTime.MinValue)
            {
                // Data di creazione non definita, la password è scaduta
                return DateTime.MinValue;
            }
            else
            {
                // Calcolo del giorno di scadenza
                return data.CreationDate.AddDays((double)this.GetAdminPasswordConfig().ValidityDays);
            }
        }

        /// <summary>
        /// Reperimento del numero di giorni di validità della password rimanenti
        /// </summary>
        /// <returns></returns>
        public int GetPasswordRemainingDays()
        {
            int remainingDays = 0;

            UserPasswordData data = this.GetPasswordData();

            if (data.PasswordNeverExpire)
            {
                // Se la password per l'utente è senza scadenza
                remainingDays = Int32.MaxValue;
            }
            else if (data.CreationDate != DateTime.MinValue)
            {
                DateTime expireDate = data.CreationDate.AddDays((double)this.GetAdminPasswordConfig().ValidityDays);
                DateTime actualDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

                if (expireDate > actualDate)
                {
                    remainingDays = expireDate.Subtract(actualDate).Days;
                }
            }

            return remainingDays;
        }

        /// <summary>
        /// Verifica se la password richiesta è uguale a quella dell'utente
        /// </summary>
        /// <param name="encryptedPassword"></param>
        /// <returns></returns>
        public bool CheckPasswordEquality(string encryptedPassword)
        {
            return (string.Compare(encryptedPassword, this.GetEncryptedPassword(), false) == 0);
        }


        /// <summary>
        /// Verifica se la password richiesta è uguale a quella dell'utente in una amministrazione
        /// </summary>
        /// <param name="encryptedPassword"></param>
        /// <returns></returns>
        public bool CheckPasswordEqualityInAmministrazione(string encryptedPassword)
        {
            return (string.Compare(encryptedPassword, this.GetEncryptedPasswordUserinAmm(), false) == 0);
        }

        /// <summary>
        /// Impostazione della password dell'utente
        /// </summary>
        /// <param name="newEncryptedPassword"></param>
        /// <param name="isAdminPassword">Se true, si inserisce la password di tipo amministratore</param>
        public void SetPassword(string newEncryptedPassword, bool isAdminPassword)
        {
            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("USER_PASSWORD_SET_PASSWORD");

                    queryDef.setParam("encryptedPassword", newEncryptedPassword);
                    
                    // Verifica se la gestione scadenza password è abilitata da amministrazione
                    //if (isAdminPassword || !this.GetAdminPasswordConfig().ExpirationEnabled)
                    //    queryDef.setParam("passwordCreationDate", "NULL");
                    //else
                    //    queryDef.setParam("passwordCreationDate", DocsPaDbManagement.Functions.Functions.ToDate(DateTime.Now.ToString("dd/MM/yyyy")));

                    // modifica effettuata per problema di cambio password su docspadmin
                    if (isAdminPassword)
                        queryDef.setParam("passwordCreationDate", "NULL");
                    else
                    {
                        if ((this.GetAdminPasswordConfig() != null && !this.GetAdminPasswordConfig().ExpirationEnabled))
                            queryDef.setParam("passwordCreationDate", DocsPaDbManagement.Functions.Functions.ToDate(DateTime.Now.ToString("dd/MM/yyyy")));
                        else
                            //queryDef.setParam("passwordCreationDate", "NULL");
                            queryDef.setParam("passwordCreationDate", DocsPaDbManagement.Functions.Functions.ToDate(DateTime.Now.ToString("dd/MM/yyyy")));
                    }

                    queryDef.setParam("userId", this.UserId);
                    //MEV utenti multi-amministrazione - aggiunto parametro idAmministrazione
                    queryDef.setParam("codAmm", this.IdAmministrazione);

                    string commandText = queryDef.getSQL();
                    logger.Debug(commandText);
                    logger.Debug(commandText);
                    if (!dbProvider.ExecuteNonQuery(commandText))
                        throw new ApplicationException(string.Format("Errore nell'impostazione della password. UserID: {0}", this.UserId));
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex);
                logger.Error(ex);
                throw ex;
            }
        }

        //MEV utenti multi-amministrazione 
        /// <summary>
        /// Impostazione della password dell'utente appartenente ad una amministrazione
        /// </summary>
        /// <param name="newEncryptedPassword"></param>
        /// <param name="isAdminPassword">Se true, si inserisce la password di tipo amministratore</param>
        public void SetPasswordForUserinAmm(string newEncryptedPassword, string modulo="")
        {
            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("USER_PASSWORD_SET_PASSWORD_FOR_USER_IN_AMM");

                    if (!string.IsNullOrEmpty(this.IdAmministrazione) && !this.IdAmministrazione.Equals("0"))
                    {
                        queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("USER_PASSWORD_SET_PASSWORD_FOR_USER_IN_AMM");
                        queryDef.setParam("encryptedPassword", newEncryptedPassword);
                        if (string.IsNullOrEmpty(modulo) || modulo != "Amministrazione")
                        {
                            queryDef.setParam("passwordCreationDate", DocsPaDbManagement.Functions.Functions.ToDate(DateTime.Now.ToString("dd/MM/yyyy")));
                        }
                        else
                        {
                            queryDef.setParam("passwordCreationDate", "null");
                        }
                        queryDef.setParam("userId", this.UserId);
                        queryDef.setParam("codAmm", this.IdAmministrazione);
                    }
                    else
                    {
                        queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("USER_PASSWORD_SET_PASSWORD");
                        queryDef.setParam("encryptedPassword", newEncryptedPassword);
                        queryDef.setParam("passwordCreationDate", DocsPaDbManagement.Functions.Functions.ToDate(DateTime.Now.ToString("dd/MM/yyyy")));

                        queryDef.setParam("userId", this.UserId);
                    }

                    string commandText = queryDef.getSQL();
                    logger.Debug(commandText);
                    logger.Debug(commandText);
                    if (!dbProvider.ExecuteNonQuery(commandText))
                        throw new ApplicationException(string.Format("Errore nell'impostazione della password. UserID: {0}", this.UserId));
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex);
                logger.Error(ex);
                throw ex;
            }
        }




        /// <summary>
        /// Impostazione della scadenza illimitata
        /// </summary>
        /// <param name="neverExpire"></param>
        /// <returns></returns>
        public void SetPasswordNeverExpireOption(bool neverExpire)
        {
            try
            {
                if (this.GetPasswordData().UserType == UserPasswordData.UserTypesEnum.User)
                {
                    using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                    {
                        DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("USER_PASSWORD_SET_PASSWORD_NEVER_EXPIRE_OPTION");

                        if (neverExpire)
                            queryDef.setParam("passwordNeverExpire", "1");
                        else
                            queryDef.setParam("passwordNeverExpire", "NULL");

                        queryDef.setParam("userId", this.UserId);

                        string commandText = queryDef.getSQL();
                        logger.Debug(commandText);
                        logger.Debug(commandText);
                        if (!dbProvider.ExecuteNonQuery(commandText))
                            throw new ApplicationException(string.Format("Errore nell'impostazione dell'opzione 'Nessuna scadenza' per la password. UserId: {0}", this.UserId));
                    }
                }
                //else
                //    throw new ApplicationException(string.Format("Impossibile impostare l'opzione 'Nessuna scadenza' per la password di un utente amministratore. UserId: {0}", this.UserId));
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="idAmministrazione"></param>
        /// <returns></returns>
        public bool PasswordNeverExpire()
        {
            return this.GetPasswordData().PasswordNeverExpire;
        }

        /// <summary>
        /// Verifica se la password è stata impostata dall'utente richiesto
        /// e non dall'amministratore
        /// </summary>
        /// <returns></returns>
        public bool IsUserPassword()
        {
            string encryptedPwd = this.GetEncryptedPassword();

            return (!string.IsNullOrEmpty(encryptedPwd) &&
                    this.GetPasswordData().CreationDate != DateTime.MinValue);
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// Utente per il quale gestire la password
        /// </summary>
        protected string UserId
        {
            get
            {
                return this._userId;
            }
        }

        /// <summary>
        /// Utente per il quale gestire la password
        /// </summary>
        protected string IdAmministrazione
        {
            get
            {
                return this._idAmministrazione;
            }
        }

        /// <summary>
        /// Reperimento password definita per l'utente
        /// </summary>
        /// <returns></returns>
        protected virtual string GetEncryptedPassword()
        {
            string password = string.Empty;

            using (DBProvider dbProvider = new DBProvider())
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("USER_PASSWORD_GET_PASSWORD");
                queryDef.setParam("userId", this.UserId);

                string commandText = queryDef.getSQL();
                logger.Debug(commandText);
                logger.Debug(commandText);
                using (IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    if (reader.Read())
                    {
                        if (!reader.IsDBNull(reader.GetOrdinal("ENCRYPTED_PASSWORD")))
                            password = reader.GetValue(reader.GetOrdinal("ENCRYPTED_PASSWORD")).ToString();

                    }
                }
            }

            return password;
        }


        /// <summary>
        /// Reperimento password definita per l'utente di una amministrazione
        /// </summary>
        /// <returns></returns>
        protected virtual string GetEncryptedPasswordUserinAmm()
        {
            string password = string.Empty;

            using (DBProvider dbProvider = new DBProvider())
            {
                //MEV utenti multi-amministrazione - rinominata la query name
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("USER_PASSWORD_GET_PASSWORD_WITHCODAMM");
                queryDef.setParam("userId", this.UserId);
                //MEV utenti multi-amministrazione - aggiunto nuovo parametro 
                queryDef.setParam("codAmm", this.IdAmministrazione);

                string commandText = queryDef.getSQL();
                logger.Debug(commandText);
                logger.Debug(commandText);
                using (IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    if (reader.Read())
                    {
                        if (!reader.IsDBNull(reader.GetOrdinal("ENCRYPTED_PASSWORD")))
                            password = reader.GetValue(reader.GetOrdinal("ENCRYPTED_PASSWORD")).ToString();

                    }
                }
            }

            return password;
        }



        /// <summary>
        /// Caricamento dati relativi alla data creazione e alla scadenza password
        /// </summary>
        protected virtual UserPasswordData GetPasswordData()
        {
            UserPasswordData data = null;

            using (DBProvider dbProvider = new DBProvider())
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("USER_PASSWORD_GET_DATA");
                queryDef.setParam("userId", this.UserId);
                //if (this.IdAmministrazione == 0)
                //    // System administrator
                //    queryDef.setParam("idAmministrazione", " IS NULL");
                //else
                //    queryDef.setParam("idAmministrazione", " = " + this.IdAmministrazione.ToString());

                string commandText = queryDef.getSQL();
                logger.Debug(commandText);
                logger.Debug(commandText);
                using (IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    if (reader.Read())
                    {
                        data = new UserPasswordData(reader);
                    }
                }
            }

            return data;
        }

        //MEV utenti multi-amministrazione
        /// <summary>
        /// Caricamento dati relativi alla data creazione e alla scadenza password per un utente di un'amministrazione
        /// </summary>
        protected virtual UserPasswordData GetPassworUserInAmmdData()
        {
            UserPasswordData data = null;

            using (DBProvider dbProvider = new DBProvider())
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("USER_IN_AMM_PASSWORD_GET_DATA");
                queryDef.setParam("userId", this.UserId);
                queryDef.setParam("codAmm", this.IdAmministrazione);

                string commandText = queryDef.getSQL();
                logger.Debug(commandText);
                logger.Debug(commandText);
                using (IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    if (reader.Read())
                    {
                        data = new UserPasswordData(reader);
                    }
                }
            }

            return data;
        }




        /// <summary>
        /// Reperimento dati di configurazione delle password per l'amministrazione
        /// </summary>
        /// <returns></returns>
        protected DocsPaVO.amministrazione.PasswordConfigurations GetAdminPasswordConfig()
        {
            if (this._passwordConfigurations == null)
                this._passwordConfigurations = AdminPasswordConfig.GetPasswordConfigurations(this.UserId);
            return this._passwordConfigurations;
        }

        //MEV utenti multi-amministrazione
        /// <summary>
        /// Reperimento dati di configurazione delle password per l'utente appartenente ad un'amministrazione 
        /// </summary>
        /// <returns></returns>
        protected DocsPaVO.amministrazione.PasswordConfigurations GetUserAdminPasswordConfig()
        {
            if (this._passwordConfigurations == null)
                this._passwordConfigurations = AdminPasswordConfig.GetPasswordConfigurations(this.UserId, this.IdAmministrazione);
            return this._passwordConfigurations;
        }



        /// <summary>
        /// 
        /// </summary>
        protected class UserPasswordData
        {
            public enum UserTypesEnum
            {
                User,
                SystemAdmin,
                SuperAdmin,
                UserAdmin
            }

            public UserPasswordData()
            {
            }

            public UserPasswordData(IDataReader reader)
            {
                if (!reader.IsDBNull(reader.GetOrdinal("PASSWORD_CREATION_DATE")))
                    this.CreationDate = reader.GetDateTime(reader.GetOrdinal("PASSWORD_CREATION_DATE"));

                if (!reader.IsDBNull(reader.GetOrdinal("PASSWORD_NEVER_EXPIRE")))
                    this.PasswordNeverExpire = (Convert.ToInt16(reader.GetValue(reader.GetOrdinal("PASSWORD_NEVER_EXPIRE"))) > 0);

                if (!reader.IsDBNull(reader.GetOrdinal("CHA_AMMINISTRATORE")))
                {
                    string tipoAmm = reader.GetValue(reader.GetOrdinal("CHA_AMMINISTRATORE")).ToString();
                    this.UserType = (UserTypesEnum)Enum.Parse(typeof(UserTypesEnum), tipoAmm, true);
                }
            }

            public DateTime CreationDate = DateTime.MinValue;
            public bool PasswordNeverExpire = false;
            public UserTypesEnum UserType = UserTypesEnum.User;
        }

        #endregion
    }
}
