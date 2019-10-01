using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Reflection;
using DocsPaVO.utente;
using DocsPaVO.Ldap;
using DocsPaVO.Validations;

namespace DocsPaLdapServices
{
    /// <summary>
    /// Classe per la gestione delle configurazioni per l'accesso da docspa ai servizi LDAP per un'amministrazione
    /// </summary>
    public sealed class LdapConfigurations
    {
        private const string LDAP_INTEGRATION_ACTIVE_KEY = "LdapIntegrationActive";
 
        /// <summary>
        /// 
        /// </summary>
        private LdapConfigurations()
        { }

        #region Public members

        /// <summary>
        /// Indica se l'integrazione LDAP è stata attivata o meno
        /// </summary>
        public static bool LdapIntegrationActivated
        {
            get
            {
                return GetConfigValue<bool>(LDAP_INTEGRATION_ACTIVE_KEY, false);
            }
        }

        /// <summary>
        /// Verifica se l'integrazione con ldap è attivata
        /// </summary>
        public static void CheckForLdapIntegrationActivated()
        {
            if (!LdapIntegrationActivated)
                throw new ApplicationException("Integrazione LDAP non attivata");
        }

        /// <summary>
        /// Reperimento configurazioni per la connessione ad un archivio LDAP
        /// </summary>
        /// <param name="idAmministrazione"></param>
        /// <returns></returns>
        public static LdapConfig GetLdapConfig(string idAmministrazione)
        {
            LdapConfig config = null;

            // Configurazione per determinare se è attiva la sincronizzazione delle utenze docspa con le utenze ldap
            bool ldapIntegrationActive = GetConfigValue<bool>(LDAP_INTEGRATION_ACTIVE_KEY, false);

            if (ldapIntegrationActive)
            {
                DocsPaDB.Query_DocsPAWS.Ldap ldapDbServices = new DocsPaDB.Query_DocsPAWS.Ldap();

                config = ldapDbServices.GetLdapConfig(idAmministrazione);
            }
            
            if (config == null)
                config = new LdapConfig();

            config.LdapIntegrationActive = ldapIntegrationActive;

            return config;
        }

        /// <summary>
        /// Memorizzazione configurazioni per la connessione ad un archivio LDAP
        /// </summary>
        /// <param name="idAmministrazione"></param>
        /// <param name="ldapInfo"></param>
        public static void SaveLdapConfig(string idAmministrazione, LdapConfig ldapInfo)
        {
            using (DocsPaDB.TransactionContext transactionalContext = new DocsPaDB.TransactionContext())
            {
                // Verifica se l'integrazione con ldap è attivata
                CheckForLdapIntegrationActivated();

                // Validazione dati di configurazione
                ValidateConfigurations(ldapInfo);

                DocsPaDB.Query_DocsPAWS.Ldap ldapDbServices = new DocsPaDB.Query_DocsPAWS.Ldap();

                ldapDbServices.SaveLdapConfig(idAmministrazione, ldapInfo);

                transactionalContext.Complete();
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
            CheckProperty<string>(ldapInfo, "DomainUserName", false, 255);
            CheckProperty<string>(ldapInfo, "DomainUserPassword", false, 255);

            if (ldapInfo.UserAttributes != null)
            {
                CheckProperty<string>(ldapInfo.UserAttributes, "UserID", true, 50);
                CheckProperty<string>(ldapInfo.UserAttributes, "Email", true, 255);
                CheckProperty<string>(ldapInfo.UserAttributes, "Matricola", true, 50);
                CheckProperty<string>(ldapInfo.UserAttributes, "Nome", true, 50);
                CheckProperty<string>(ldapInfo.UserAttributes, "Cognome", true, 50);
                CheckProperty<string>(ldapInfo.UserAttributes, "Sede", true, 255);
            }
            else
                throw new ApplicationException("Nessun valore specificato per i campi LDAP");
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