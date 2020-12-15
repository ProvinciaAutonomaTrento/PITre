using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Reflection;
using DocsPaVO.utente;
using DocsPaVO.Ldap;
using DocsPaVO.Validations;

namespace DocsPaLdapServices
{
    /// <summary>
    /// Classe per la gestione delle configurazioni per l'accesso da docspa ai servizi LDAP per un utente
    /// </summary>
    public sealed class LdapUserConfigurations
    {
        /// <summary>
        /// 
        /// </summary>
        private LdapUserConfigurations()
        { }

        #region Public members

        /// <summary>
        /// Reperimento configurazioni LDAP impostate per un utente
        /// </summary>
        /// <param name="idUser"></param>
        /// <returns></returns>
        public static LdapUserConfig GetLdapUserConfig(string idUser)
        {
            DocsPaVO.utente.Utente user = null;

            using (DocsPaDB.Query_DocsPAWS.Utenti utentiDb = new DocsPaDB.Query_DocsPAWS.Utenti())
                user = utentiDb.getUtenteById(idUser);

            return GetLdapUserConfigByName(user.userId);
        }

        /// <summary>
        /// Reperimento configurazioni LDAP impostate per un utente
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static LdapUserConfig GetLdapUserConfigByName(string userName)
        {
            LdapUserConfig config = null;

            // Configurazione per determinare se è attiva la sincronizzazione delle utenze docspa con le utenze ldap
            bool ldapIntegrationActive = LdapConfigurations.LdapIntegrationActivated;

            if (ldapIntegrationActive)
            {
                DocsPaDB.Query_DocsPAWS.Ldap ldapDbServices = new DocsPaDB.Query_DocsPAWS.Ldap();

                config = ldapDbServices.GetLdapUserConfigByName(userName);
            }

            if (config == null)
                config = new LdapUserConfig();

            return config;
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="idUser"></param>
        ///// <param name="ldapInfo"></param>
        //public static void SaveLdapUserConfig(string idUser, LdapConfig ldapInfo)
        //{
        //    using (DocsPaDB.TransactionContext transactionalContext = new DocsPaDB.TransactionContext())
        //    {
        //        // Verifica se l'integrazione con ldap è attivata
        //        LdapConfigurations.CheckForLdapIntegrationActivated();

        //        // Validazione dati di configurazione
        //        ValidateConfigurations(ldapInfo);

        //        DocsPaDB.Query_DocsPAWS.Ldap ldapDbServices = new DocsPaDB.Query_DocsPAWS.Ldap();

        //        ldapDbServices.SaveLdapUserConfig(idUser, ldapInfo);

        //        transactionalContext.Complete();
        //    }
        //}

        /// <summary>
        /// Verifica se un determinato utente può collegarsi ad un archivio LDAP
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static bool UserCanConnectToLdap(string userName)
        {
            string idAmministrazioneAsString;
            using (DocsPaDB.Query_DocsPAWS.Utenti utentiDb = new DocsPaDB.Query_DocsPAWS.Utenti())
                utentiDb.GetIdAmmUtente(out idAmministrazioneAsString, userName);

            int idAmministrazione;
            Int32.TryParse(idAmministrazioneAsString, out idAmministrazione);

            if (idAmministrazione > 0)
            {
                // Solo gli utenti configurati in un amministrazione posso autenticarsi ad LDAP
                DocsPaVO.Ldap.LdapConfig configInfo = DocsPaLdapServices.LdapConfigurations.GetLdapConfig(idAmministrazioneAsString);

                DocsPaVO.Ldap.LdapUserConfig userConfigInfo = GetLdapUserConfigByName(userName);

                return (!string.IsNullOrEmpty(configInfo.ServerName) && !string.IsNullOrEmpty(configInfo.GroupDN) &&
                                    userConfigInfo.LdapAuthenticated);
            }
            else
            {
                // Utente non associato ad un'amministrazione (es. SuperAdmin)
                return false;
            }
        }

        #endregion

        #region Private members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ldapInfo"></param>
        /// <returns></returns>
        private static void ValidateConfigurations(LdapConfig ldapInfo)
        {
            if (ldapInfo == null)
                throw new ApplicationException("Configurazioni LDAP non specificate");

            CheckProperty<string>(ldapInfo, "ServerName", true, 255);
            CheckProperty<string>(ldapInfo, "GroupDN", true, 255);
        }

        /// <summary>
        /// Validazione proprietà in base ai parametri forniti definiti 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <param name="required"></param>
        /// <param name="maxLenght"></param>        
        private static void CheckProperty<T>(object obj, string propertyName, bool required, int maxLenght)
        {
            FieldInfo p = obj.GetType().GetField(propertyName);

            if (p != null)
            {
                T value = (T)p.GetValue(obj);

                if (required)
                {
                    bool isNull = false;

                    if (typeof(T) == typeof(string))
                        isNull = string.IsNullOrEmpty((string)Convert.ChangeType(value, TypeCode.String));
                    else
                        isNull = value.Equals(default(T));

                    if (isNull)
                        throw new ApplicationException(string.Format("{0}: dato obbligatorio", p.Name));

                }

                // Verifica dimensione massima campo
                if (p.FieldType.Equals(typeof(string)) && maxLenght > 0 &&
                    value != null && value.ToString().Length > maxLenght)
                    throw new ApplicationException(string.Format("La dimensione massima per il campo {0} è di {1} caratteri", p.Name, maxLenght.ToString()));
            }
        }

        /// <summary>
        /// Reperimento del valore di una configurazione
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configKey"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        private static T GetConfigValue<T>(string configKey, T defaultValue)
        {
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings[configKey]))
                return (T)Convert.ChangeType(ConfigurationManager.AppSettings[configKey], typeof(T));
            else
                return defaultValue;
        }

        #endregion 
    }
}