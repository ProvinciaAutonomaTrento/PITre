using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.DirectoryServices;
using System.DirectoryServices.Protocols;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using DocsPaVO.Ldap;
using log4net;

namespace DocsPaLdapServices.Core
{
    /// <summary>
    /// Classe che espone dei servizi per la gestione di oggetti 'user' in LDAP via SSL
    /// </summary>
    public class LdapUserServicesSSL : BaseLdapUserServices
    {
        private ILog logger = LogManager.GetLogger(typeof(LdapUserServicesSSL));
        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        public LdapUserServicesSSL(LdapConfig info)
            : base(info)
        { }

        #region Public members

        /// <summary>
        /// Ricerca di oggetti di tipo "user" contenuti in un particolare gruppo di LDAP
        /// </summary>
        /// <returns></returns>
        public override LdapUser[] GetUsers()
        {
           
            List<LdapUser> users = new List<LdapUser>();

            using (LdapConnection con = this.CreateLdapConnection())
            {
                
                try
                {
                    con.Bind();
                    
                    SearchRequest request = new SearchRequest(this.Info.GroupDN, "(objectClass=*)", System.DirectoryServices.Protocols.SearchScope.Subtree);//"(objectClass=group)"
                   
                    SearchResponse response = (SearchResponse)con.SendRequest(request);
          
                    foreach (SearchResultEntry entry in response.Entries)
                    {
                        // L'attributo "member" contiene i DN di tutti gli utenti del gruppo
                        DirectoryAttribute attribute = entry.Attributes["member"];

                        for (int i = 0; i < attribute.Count; i++)
                        {
                            // Ogni occorrenza della collection "attribute" corrisponde ad un DN dell'utente appartenente al gruppo

                            string dn = string.Empty;

                            // Reperimento del distinguished name
                            byte[] bytes = attribute[i] as byte[];

                            if (bytes != null)
                                // Presenza di caratteri che i directoryservices non riescono a descriptare (es. accentanti)
                                dn = ASCIIEncoding.Default.GetString(bytes);
                            else
                                dn = attribute[i].ToString();

                            // Ricerca singolo utente
                            SearchRequest requestUser = new SearchRequest(dn,
                                        "(objectClass=*)", //"(objectClass=person)",
                                        System.DirectoryServices.Protocols.SearchScope.Subtree);

                            SearchResponse responseUser = (SearchResponse)con.SendRequest(requestUser);

                            foreach (SearchResultEntry entryUser in responseUser.Entries)
                            {
                                users.Add(this.CreateLdapUserInstance(entryUser));
                            }
                        }
                    }
                }
                catch (LdapException ex)
                {
                    throw new ApplicationException("Percorso LDAPS o credenziali di dominio non valide: "  + ex.Message);
                }
                catch (DirectoryOperationException ex2)
                {
                    throw new ApplicationException("Credenziali di dominio non valide: " + ex2.Message);
                }
            }

            return users.ToArray();
        }

        /// <summary>
        /// Autenticazione utente ad LDAP
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public override bool AuthenticateUser(string userName, string password)
        {
            bool retValue = false;
            logger.DebugFormat("AuthenticateUser SSL -> uid {0}  pass {1}", userName, password);
            using (LdapConnection con = this.CreateLdapConnection(userName, password))
            {
                try
                {
                    con.Bind();

                    retValue = true;
                }
                catch (Exception ex)
                {
                    logger.Debug("Percorso LDAP o credenziali di dominio non valide", ex);

                    retValue = false;
                }
            }

            return retValue;
        }

        #endregion

        #region Protected members

        /// <summary>
        /// Path del certificato utilizzato per autenticarsi al server LDAP via SSL
        /// </summary>
        protected string CertificatePath
        {
            get
            {
                const string CONFIG_KEY = "LdapUserSyncCertificatePath";
                if (string.IsNullOrEmpty(ConfigurationManager.AppSettings[CONFIG_KEY]))
                    throw new ApplicationException("Percorso del certificato per l'autenticazione ad LDAP non impostato");
                return ConfigurationManager.AppSettings[CONFIG_KEY];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entryUser"></param>
        /// <param name="propertyName"></param>
        /// <param name="nullable"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        protected string GetPropertyValue(SearchResultEntry entryUser, string propertyName, bool nullable, string defaultValue)
        {
            if (!nullable && !entryUser.Attributes.Contains(propertyName))
                throw new ApplicationException(string.Format("Valore richiesto per la proprietà '{0}' dell'utente '{1}' in LDAP", propertyName, GetPropertyValue(entryUser, "distinguishedName", true, string.Empty)));
            else
            {
                if (!entryUser.Attributes.Contains(propertyName))
                    return defaultValue;
                else
                {
                    byte[] bytes = entryUser.Attributes[propertyName][0] as byte[];

                    if (bytes != null)
                        return Encoding.Default.GetString(bytes);
                    else
                        return entryUser.Attributes[propertyName][0].ToString();
                }
            }
        }

        /// <summary>
        /// Creazione di una nuova connessione ad ldap tramite le credenziali di un utente fornite come parametri
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        protected virtual LdapConnection CreateLdapConnection(string userName, string password)
        {
            logger.DebugFormat("Inizio Connession tramite SSL uid: {0} pass {1}", userName, password);
            LdapConnection con = new LdapConnection(new LdapDirectoryIdentifier(this.Info.Host));
            con.SessionOptions.SecureSocketLayer = true;
            con.SessionOptions.ProtocolVersion = 3;
            con.SessionOptions.VerifyServerCertificate = new VerifyServerCertificateCallback(this.OnServerAuthenticate);
            con.Credential = new NetworkCredential(userName, password);
            con.AuthType = AuthType.Basic;
            return con;
        }

        /// <summary>
        /// Creazione di una nuova connessione ad ldap
        /// </summary>
        /// <returns></returns>
        protected virtual LdapConnection CreateLdapConnection()
        {
            return this.CreateLdapConnection(this.Info.DomainUserName, this.Info.DomainUserPassword);
        }

        /// <summary>
        /// Creazione di un'istanza dell'utente in LDAP contenente gli attributi significativi richiesti
        /// </summary>
        /// <param name="entryUser"></param>
        /// <returns></returns>
        protected LdapUser CreateLdapUserInstance(SearchResultEntry entryUser)
        {
            LdapUser ldapUser = new LdapUser();

            try
            {
                //ldapUser.DN = this.GetPropertyValue(entryUser, "distinguishedName", false, string.Empty);
                ldapUser.DN = entryUser.DistinguishedName;
                ldapUser.UserID = this.GetPropertyValue(entryUser, this.Info.UserAttributes.UserId, false, string.Empty);
                ldapUser.Matricola = this.GetPropertyValue(entryUser, this.Info.UserAttributes.Matricola, false, string.Empty);
                ldapUser.Email = this.GetPropertyValue(entryUser, this.Info.UserAttributes.Email, false, string.Empty);
                ldapUser.Nome = this.GetPropertyValue(entryUser, this.Info.UserAttributes.Nome, false, string.Empty);
                ldapUser.Cognome = this.GetPropertyValue(entryUser, this.Info.UserAttributes.Cognome, false, string.Empty);
                ldapUser.Sede = this.GetPropertyValue(entryUser, this.Info.UserAttributes.Sede, true, string.Empty);
            }
            catch (Exception ex)
            {
                logger.Debug(ex);

                ldapUser.ErrorMessage = ex.Message;
            }

            return ldapUser;
        }

        /// <summary>
        /// Handler per l'evento VerifyServerCertificateCallback per la verifica
        /// delle credenziali per l'accesso al server LDAP tramite certificato
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="certificate"></param>
        /// <returns></returns>
        protected virtual bool OnServerAuthenticate(LdapConnection connection, X509Certificate certificate)
        {
            
            string certPath = this.CertificatePath;
            if ((certPath.ToLower() == "false") ||
               (certPath.ToLower() == "0"))
                return true;

            try
            {
                // Reperimento del certificato configurato
                X509Certificate expectedCert = X509Certificate.CreateFromCertFile(this.CertificatePath);

                if (expectedCert.Equals(certificate))
                {
                    return true;
                }
                else
                {
                    // certificate.ToString(true) provides verbose information about the certificate

                    string errorMessage =
                        String.Format(
                        "Il certificato non corrisponde con quello restituito dal server: {0}",
                        certificate.ToString(true));

                    logger.Debug(errorMessage);

                    return false;
                }
            }
            catch (Exception ex)
            {
                logger.Debug("Cannot validate certificate: " + ex.Message);
                return false;
            }
        }

        #endregion
    }
}