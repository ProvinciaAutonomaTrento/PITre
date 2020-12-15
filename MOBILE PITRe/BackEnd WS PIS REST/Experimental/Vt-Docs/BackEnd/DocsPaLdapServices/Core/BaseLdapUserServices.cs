using System;
using System.Collections.Generic;
using System.Text;
using DocsPaVO.Ldap;

namespace DocsPaLdapServices.Core
{
    /// <summary>
    /// Class factory per la creazione di un'istanza specifica per l'accesso ai servizi DirectoryServices
    /// </summary>
    public sealed class LdapUserServicesFactory
    {
        /// <summary>
        /// 
        /// </summary>
        private LdapUserServicesFactory()
        { }

        /// <summary>
        /// Factory method servizi LDAP per una determinata amministrazione
        /// </summary>
        /// <param name="idAmministrazione"></param>
        /// <returns></returns>
        public static BaseLdapUserServices GetConfiguredInstance(string idAmministrazione)
        {
            // Reperimento configurazioni LDAP per l'amministrazione richiesta
            LdapConfig config = LdapConfigurations.GetLdapConfig(idAmministrazione);

            return CreateInstance(config);
        }

        /// <summary>
        /// Factory method
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static BaseLdapUserServices CreateInstance(LdapConfig info)
        {
            if (info.SSL)
                // Creazione istanza per l'accesso ad LDAP tramite SSL
                return new LdapUserServicesSSL(info);
            else
                return new LdapUserServices(info);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public abstract class BaseLdapUserServices
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        public BaseLdapUserServices(LdapConfig info)
        {
            this.Info = info;
        }

        #region Public members

        /// <summary>
        /// Ricerca di oggetti di tipo "user" contenuti in un particolare gruppo di LDAP
        /// </summary>
        /// <returns></returns>
        public abstract LdapUser[] GetUsers();

        /// <summary>
        /// Verifica credenziali per l'autenticazione ad un particolare gruppo LDAP
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public abstract bool AuthenticateUser(string userName, string password);

        #endregion

        #region Protected members

        /// <summary>
        /// ClassTypes
        /// </summary>
        protected const string OBJECT_TYPE_USER = "user";
        protected const string OBJECT_TYPE_GROUP = "group";
        protected const string OBJECT_TYPE_CONTAINER = "container";
        protected const string OBJECT_TYPE_ORGANIZATIONAL_UNIT = "organizationalUnit";
        protected const string OBJECT_TYPE_ORGANIZATION = "organization";
        protected const string OBJECT_TYPE_DOMAIN_DNS = "domainDNS";

        /// <summary>
        /// Informazioni per l'integrazione con i servizi LDAP
        /// </summary>
        protected LdapConfig Info { get; set; }

        #endregion
    }
}
