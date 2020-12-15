using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Xml.Serialization;
using System.Security.Cryptography;
using System.IO;
using DocsPaVO.Ldap;
using DocsPaVO.utente;
using log4net;

namespace DocsPaDB.Query_DocsPAWS
{
    /// <summary>
    /// Classe per la gestione dell'accesso ai dati per l'integrazione con ldap
    /// </summary>
    public class Ldap
    {
        private ILog logger = LogManager.GetLogger(typeof(Ldap));
        /// <summary>
        /// 
        /// </summary>
        public Ldap()
        { }

        #region Public members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idAmministrazione"></param>
        /// <param name="userName"></param>
        /// <param name="clearPassword"></param>
        public void SetLdapConfigDomainCredentials(string idAmministrazione, string userName, string clearPassword)
        {
            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("U_SET_LDAP_CONFIG_DOMAIN_CREDENTIALS");

                    queryDef.setParam("userName", this.NormalizeStringValue(userName));

                    string encryptedPassword = string.Empty;
                    clearPassword = this.NormalizeStringValue(clearPassword);
                    if (!string.IsNullOrEmpty(clearPassword))
                        encryptedPassword = PasswordEncrpytorHelper.Encrypt(clearPassword);

                    queryDef.setParam("encryptedPassword", encryptedPassword);

                    queryDef.setParam("idAmm", idAmministrazione);

                    string commandText = queryDef.getSQL();
                    logger.Debug(commandText);

                    int rowsAffected;
                    if (!dbProvider.ExecuteNonQuery(commandText, out rowsAffected))
                        throw new ApplicationException("Errore in esecuzione query U_SET_LDAP_CONFIG_DOMAIN_CREDENTIALS");
                }
            }
            catch (Exception ex)
            {
                string errorMessage = "Errore nell'aggiornamento delle credenziali dell'utente di dominio LDAP per l'amministrazione";

                logger.Debug(errorMessage, ex);
                throw new ApplicationException(errorMessage, ex);
            }
        }

        /// <summary>
        /// Reperimento password configurata per l'utente di dominio nell'ambito dell'autenticazione ad ldap
        /// </summary>
        /// <param name="idAmministrazione"></param>
        /// <returns></returns>
        public string GetLdapConfigDomainUserPassword(string idAmministrazione)
        {
            try
            {
                string userPassword = string.Empty;

                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_LDAP_CONFIG_DOMAIN_USER_PASSWORD");
                    queryDef.setParam("idAmm", idAmministrazione);

                    string commandText = queryDef.getSQL();
                    logger.Debug(commandText);

                    string field;
                    if (dbProvider.ExecuteScalar(out field, commandText))
                    {
                        if (!string.IsNullOrEmpty(field))
                            userPassword = PasswordEncrpytorHelper.Decrypt(field);
                    }
                    else
                        throw new ApplicationException("Errore in esecuzione query S_GET_LDAP_CONFIG_DOMAIN_USER_PASSWORD");
                }

                return userPassword;
            }
            catch (Exception ex)
            {
                string errorMessage = "Errore nel reperimento della password dell'utente di dominio LDAP per l'amministrazione";

                logger.Debug(errorMessage, ex);
                throw new ApplicationException(errorMessage, ex);
            }
        }

        /// <summary>
        /// Reperimento delle informazioni per l'integrazione con ldap per una singola amministrazione
        /// </summary>
        /// <param name="idAmministrazione"></param>
        /// <returns></returns>
        public DocsPaVO.Ldap.LdapConfig GetLdapConfig(string idAmministrazione)
        {
            try
            {
                DocsPaVO.Ldap.LdapConfig info = null;

                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_LDAP_CONFIG");
                    queryDef.setParam("idAmm", idAmministrazione);

                    string commandText = queryDef.getSQL();
                    logger.Debug(commandText);

                    using (IDataReader reader = dbProvider.ExecuteReader(commandText))
                    {
                        if (reader.Read())
                        {
                            info = new LdapConfig();

                            info.ServerName = DocsPaUtils.Data.DataReaderHelper.GetValue<string>(reader, "SERVER_NAME", false);
                            info.DomainUserName = DocsPaUtils.Data.DataReaderHelper.GetValue<string>(reader, "USER_NAME", true, string.Empty);
                            info.GroupDN = DocsPaUtils.Data.DataReaderHelper.GetValue<string>(reader, "GROUP_DN", false);

                            info.UserAttributes = new LdapUserAttributes();
                            info.UserAttributes.UserId = DocsPaUtils.Data.DataReaderHelper.GetValue<string>(reader, "USERID_ATTRIBUTE", true, string.Empty);
                            info.UserAttributes.Email = DocsPaUtils.Data.DataReaderHelper.GetValue<string>(reader, "EMAIL_ATTRIBUTE", true, string.Empty);
                            info.UserAttributes.Matricola = DocsPaUtils.Data.DataReaderHelper.GetValue<string>(reader, "MATRICOLA_ATTRIBUTE", true, info.UserAttributes.UserId);
                            info.UserAttributes.Nome = DocsPaUtils.Data.DataReaderHelper.GetValue<string>(reader, "NOME_ATTRIBUTE", true, string.Empty);
                            info.UserAttributes.Cognome = DocsPaUtils.Data.DataReaderHelper.GetValue<string>(reader, "COGNOME_ATTRIBUTE", true, string.Empty);
                            info.UserAttributes.Sede = DocsPaUtils.Data.DataReaderHelper.GetValue<string>(reader, "SEDE_ATTRIBUTE", true, string.Empty);
                        }
                    }
                }

                return info;
            }
            catch (Exception ex)
            {
                string errorMessage = "Errore nel reperimento delle configurazione LDAP per l'amministrazione";

                logger.Debug(errorMessage, ex);
                throw new ApplicationException(errorMessage, ex);
            }
        }

        /// <summary>
        /// Reperimento delle informazioni di configurazione ldap per una singolo utente
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public DocsPaVO.Ldap.LdapUserConfig GetLdapUserConfigByName(string userName)
        {
            try
            {
                DocsPaVO.Ldap.LdapUserConfig info = null;

                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_LDAP_USER_CONFIG_BY_NAME");
                    queryDef.setParam("userName", userName);

                    string commandText = queryDef.getSQL();
                    logger.Debug(commandText);

                    using (IDataReader reader = dbProvider.ExecuteReader(commandText))
                    {
                        if (reader.Read())
                        {
                            info = new LdapUserConfig
                            {
                                LdapIdSync = DocsPaUtils.Data.DataReaderHelper.GetValue<string>(reader, "LDAP_ID_SYNC", true, string.Empty),
                                LdapSyncronized = (!string.IsNullOrEmpty(DocsPaUtils.Data.DataReaderHelper.GetValue<string>(reader, "LDAP_ID_SYNC", true, string.Empty)) &&
                                                   (DocsPaUtils.Data.DataReaderHelper.GetValue<string>(reader, "LDAP_NEVER_SYNC", true, "0") == "0")),
                                LdapAuthenticated = (DocsPaUtils.Data.DataReaderHelper.GetValue<string>(reader, "LDAP_AUTHENTICATED", true, "0") == "1")
                            };

                            //if (info.LdapAuthenticated)
                            //{
                            //    info.LdapUserConfigSettings = new LdapConfig
                            //    {
                            //        LdapIntegrationActive = true,
                            //        ServerName = DocsPaUtils.Data.DataReaderHelper.GetValue<string>(reader, "SERVER_NAME", true, string.Empty),
                            //        GroupDN = DocsPaUtils.Data.DataReaderHelper.GetValue<string>(reader, "GROUP_DN", true, string.Empty)
                            //    };

                            //    if (string.IsNullOrEmpty(info.LdapUserConfigSettings.ServerName) ||
                            //        string.IsNullOrEmpty(info.LdapUserConfigSettings.GroupDN))
                            //    {
                            //        using (Utenti userDb = new Utenti())
                            //        {
                            //            string idAmm;

                            //            // Reperimento id amministrazione per l'utente
                            //            userDb.GetIdAmmUtente(out idAmm, userName);

                            //            LdapConfig adminConfig = GetLdapConfig(idAmm);
                            //            info.LdapUserConfigSettings.ServerName = adminConfig.ServerName;
                            //            info.LdapUserConfigSettings.GroupDN = adminConfig.GroupDN;
                            //        }
                            //    }
                            //}
                        }
                    }
                }

                return info;
            }
            catch (Exception ex)
            {
                string errorMessage = "Errore nel reperimento delle configurazione LDAP per l'utente";

                logger.Debug(errorMessage, ex);
                throw new ApplicationException(errorMessage, ex);
            }
        }

        /// <summary>
        /// Save delle informazioni per l'integrazione docspa / ldap
        /// </summary>
        /// <param name="idAmministrazione"></param>
        /// <param name="ldapInfo"></param>
        public void SaveLdapConfig(string idAmministrazione, LdapConfig ldapInfo)
        {
            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_CONTAINS_LDAP_CONFIG");
                    queryDef.setParam("idAmm", idAmministrazione);

                    string commandText = queryDef.getSQL();
                    logger.Debug(commandText);

                    string field;

                    if (dbProvider.ExecuteScalar(out field, commandText))
                    {
                        string queryName = string.Empty;

                        if (Convert.ToInt32(field) > 0)
                            queryName = "U_UPDATE_LDAP_CONFIG";
                        else
                            queryName = "I_INSERT_LDAP_CONFIG";

                        queryDef = DocsPaUtils.InitQuery.getInstance().getQuery(queryName);

                        queryDef.setParam("idAmm", idAmministrazione);
                        queryDef.setParam("serverName", this.NormalizeStringValue(ldapInfo.ServerName));
                        queryDef.setParam("groupDN", this.NormalizeStringValue(ldapInfo.GroupDN));
                        queryDef.setParam("userId", this.NormalizeStringValue(ldapInfo.UserAttributes.UserId));
                        queryDef.setParam("email", this.NormalizeStringValue(ldapInfo.UserAttributes.Email));
                        queryDef.setParam("matricola", this.NormalizeStringValue(ldapInfo.UserAttributes.Matricola));
                        queryDef.setParam("nome", this.NormalizeStringValue(ldapInfo.UserAttributes.Nome));
                        queryDef.setParam("cognome", this.NormalizeStringValue(ldapInfo.UserAttributes.Cognome));
                        queryDef.setParam("sede", this.NormalizeStringValue(ldapInfo.UserAttributes.Sede));

                        commandText = queryDef.getSQL();
                        logger.Debug(commandText);

                        int rowsAffected;
                        if (!dbProvider.ExecuteNonQuery(commandText, out rowsAffected))
                            throw new ApplicationException(string.Format("Errore in esecuzione query {0}", queryName));
                        else
                        {
                            if (!string.IsNullOrEmpty(ldapInfo.DomainUserName) && !string.IsNullOrEmpty(ldapInfo.DomainUserPassword))
                                this.SetLdapConfigDomainCredentials(idAmministrazione, ldapInfo.DomainUserName, ldapInfo.DomainUserPassword);
                        }
                    }
                    else
                        throw new ApplicationException("Errore in esecuzione query S_CONTAINS_LDAP_CONFIG");
                }
            }
            catch (Exception ex)
            {
                string errorMessage = "Errore nell'aggiornamento delle configurazione LDAP per l'amministrazione";

                logger.Debug(errorMessage, ex);
                throw new ApplicationException(errorMessage, ex);
            }
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="idUser"></param>
        ///// <param name="ldapInfo"></param>
        //public void SaveLdapUserConfig(string idUser, LdapConfig ldapInfo)
        //{
        //    try
        //    {
        //        using (DocsPaDB.DBProvider dbProvider = new DBProvider())
        //        {
        //            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_CONTAINS_LDAP_USER_CONFIG");
        //            queryDef.setParam("idUser", idUser);

        //            string commandText = queryDef.getSQL();
        //            logger.Debug(commandText);

        //            string field;

        //            if (dbProvider.ExecuteScalar(out field, commandText))
        //            {
        //                string queryName = string.Empty;

        //                if (Convert.ToInt32(field) > 0)
        //                    queryName = "U_UPDATE_LDAP_USER_CONFIG";
        //                else
        //                    queryName = "I_INSERT_LDAP_USER_CONFIG";

        //                queryDef = DocsPaUtils.InitQuery.getInstance().getQuery(queryName);

        //                queryDef.setParam("idUser", idUser);
        //                queryDef.setParam("serverName", this.NormalizeStringValue(ldapInfo.ServerName));
        //                queryDef.setParam("groupDN", this.NormalizeStringValue(ldapInfo.GroupDN));

        //                commandText = queryDef.getSQL();
        //                logger.Debug(commandText);

        //                int rowsAffected;
        //                if (!dbProvider.ExecuteNonQuery(commandText, out rowsAffected))
        //                    throw new ApplicationException(string.Format("Errore in esecuzione query {0}", queryName));
        //            }
        //            else
        //                throw new ApplicationException("Errore in esecuzione query S_CONTAINS_LDAP_USER_CONFIG");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string errorMessage = "Errore nell'aggiornamento delle configurazione LDAP per l'utente";

        //        logger.Debug(errorMessage, ex);
        //        throw new ApplicationException(errorMessage, ex);
        //    }
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idAmministrazione"></param>
        /// <returns></returns>
        public LdapSyncronizationHistoryItem[] GetLdapSyncHistory(string idAmministrazione)
        {
            try
            {
                List<LdapSyncronizationHistoryItem> result = new List<LdapSyncronizationHistoryItem>();

                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_LDAP_SYNC_HISTORY");
                    queryDef.setParam("idAmm", idAmministrazione);

                    string commandText = queryDef.getSQL();
                    logger.Debug(commandText);

                    using (IDataReader reader = dbProvider.ExecuteReader(commandText))
                    {
                        while (reader.Read())
                        {
                            LdapSyncronizationHistoryItem item = new LdapSyncronizationHistoryItem();

                            item.Id = Convert.ToInt32(DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "SYSTEM_ID", false));
                            item.User = DocsPaUtils.Data.DataReaderHelper.GetValue<string>(reader, "USER_ID", false);
                            item.Date = DocsPaUtils.Data.DataReaderHelper.GetValue<DateTime>(reader, "SYNC_DATE", false);
                            item.ItemsSyncronized = Convert.ToInt32(DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "ITEMS_SYNCRONIZED", false, 0));
                            item.ErrorDetails = DocsPaUtils.Data.DataReaderHelper.GetValue<string>(reader, "ERROR_DETAILS", true, string.Empty);

                            result.Add(item);
                        }
                    }
                }

                return result.ToArray();
            }
            catch (Exception ex)
            {
                string errorMessage = "Errore nel reperimento delle informazioni sulle sincronizzazioni LDAP effettuate per l'amministrazione";

                logger.Debug(errorMessage, ex);
                throw new ApplicationException(errorMessage, ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="historyId"></param>
        public void DeleteLdapSyncHistoryItem(int idHistoryItem)
        {
            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("D_DELETE_LDAP_SYNC_HISTORY_ITEM");
                    queryDef.setParam("id", idHistoryItem.ToString());

                    string commandText = queryDef.getSQL();
                    logger.Debug(commandText);

                    int rowsAffected;
                    dbProvider.ExecuteNonQuery(commandText, out rowsAffected);

                    if (rowsAffected == 0)
                        throw new ApplicationException("Errore in esecuzione query D_DELETE_LDAP_SYNC_HISTORY_ITEM");
                }
            }
            catch (Exception ex)
            {
                string errorMessage = "Errore nella rimozione delle informazioni sulle sincronizzazioni LDAP effettuate per l'amministrazione";

                logger.Debug(errorMessage, ex);
                throw new ApplicationException(errorMessage, ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idAmministrazione"></param>
        /// <param name="syncResponse"></param>        
        /// <returns></returns>
        public LdapSyncronizationHistoryItem SaveLdapSyncResult(string idAmministrazione, LdapSyncronizationResponse syncResponse)
        {
            try
            {
                LdapSyncronizationHistoryItem result = null;

                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    dbProvider.BeginTransaction();

                    const string TABLE_NAME = "DPA_LDAP_SYNC_HISTORY";

                    DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("I_INSERT_LDAP_SYNC_HISTORY_ITEM");

                    queryDef.setParam("colId", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                    queryDef.setParam("idValue", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal(TABLE_NAME));
                    queryDef.setParam("idAmm", idAmministrazione);
                    queryDef.setParam("userId", this.NormalizeStringValue(syncResponse.User));
                    queryDef.setParam("syncDate", DocsPaDbManagement.Functions.Functions.GetDate());
                    queryDef.setParam("itemsSync", syncResponse.ItemsSyncronized.ToString());
                    queryDef.setParam("errorDetails", this.NormalizeStringValue(syncResponse.ErrorDetails));

                    string commandText = queryDef.getSQL();
                    logger.Debug(commandText);

                    int rowsAffected;
                    if (dbProvider.ExecuteNonQuery(commandText, out rowsAffected))
                    {
                        if (rowsAffected == 1)
                        {
                            commandText = DocsPaDbManagement.Functions.Functions.GetQueryLastSystemIdInserted(TABLE_NAME);
                            logger.Debug(commandText);

                            string field;
                            if (dbProvider.ExecuteScalar(out field, commandText))
                            {
                                result = new LdapSyncronizationHistoryItem();
                                result.Id = Convert.ToInt32(field);
                                result.User = syncResponse.User;
                                result.Date = syncResponse.Date;
                                result.ErrorDetails = syncResponse.ErrorDetails;

                                dbProvider.CommitTransaction();
                            }
                        }

                    }
                    else
                        throw new ApplicationException("Errore in esecuzione query I_INSERT_LDAP_SYNC_HISTORY_ITEM");
                }

                return result;
            }
            catch (Exception ex)
            {
                string errorMessage = "Errore nell'aggiornamento delle informazioni sulla sincronizzazione LDAP effettuata per l'amministrazione";

                logger.Debug(errorMessage, ex);
                throw new ApplicationException(errorMessage, ex);
            }
        }

        #endregion

        #region Protected members

        /// <summary>
        /// Normalizzazione valore stringa per database
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected string NormalizeStringValue(string value)
        {
            if (value != null)
                return value.Replace("'", "''").Trim();
            else
                return string.Empty;
        }

        #endregion

        #region Private members

        /// <summary>
        /// 
        /// </summary>
        private sealed class PasswordEncrpytorHelper
        {
            // per ora lasciamo le chiavi fisse
            static byte[] key = { 145, 12, 32, 245, 98, 132, 98, 214, 6, 77, 131, 44, 221, 3, 9, 50 };
            static byte[] iv = { 15, 122, 132, 5, 93, 198, 44, 31, 9, 39, 241, 49, 250, 188, 80, 7 };


            public static string Encrypt(string str)
            {
                System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
                return BytesToHex(Encrypt(encoding.GetBytes(str)));
            }

            public static byte[] Encrypt(byte[] data)
            {
                return Encrypt(data, key, iv);
            }

            public static byte[] Encrypt(byte[] data, byte[] key, byte[] iv)
            {
                using (Rijndael algorithm = Rijndael.Create())
                using (ICryptoTransform encryptor = algorithm.CreateEncryptor(key, iv))
                    return Crypta(data, key, iv, encryptor);
            }

            public static string Decrypt(string hexstr)
            {
                System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
                return encoding.GetString(Decrypt(HexToBytes(hexstr)));
            }

            public static byte[] Decrypt(byte[] data)
            {
                return Decrypt(data, key, iv);
            }

            public static byte[] Decrypt(byte[] data, byte[] key, byte[] iv)
            {
                using (Rijndael algorithm = Rijndael.Create())
                using (ICryptoTransform decryptor = algorithm.CreateDecryptor(key, iv))
                    return Crypta(data, key, iv, decryptor);
            }

            public static byte[] Crypta(byte[] data, byte[] key, byte[] iv,
                                 ICryptoTransform cryptor)
            {
                MemoryStream m = new MemoryStream();
                using (Stream c = new CryptoStream(m, cryptor, CryptoStreamMode.Write))
                    c.Write(data, 0, data.Length);
                return m.ToArray();
            }

            public static String BytesToHex(byte[] data)
            {
                string st = "";
                for (int i = 0; i < data.Length; i++)
                    st += data[i].ToString("X2");
                return st;
            }

            public static byte[] HexToBytes(string data)
            {
                int l = data.Length / 2;
                byte[] bytes = new byte[l];
                for (int i = 0; i < l; i++)
                    bytes[i] = byte.Parse(data.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber);
                return bytes;
            }
        }

        #endregion
    }
}
