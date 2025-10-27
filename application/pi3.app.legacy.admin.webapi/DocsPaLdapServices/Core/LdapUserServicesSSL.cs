// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.Ldap;
using Novell.Directory.Ldap;
using Serilog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace DocsPaLdapServices.Core;

/// <summary>
/// Classe che espone dei servizi per la gestione di oggetti 'user' in LDAP via SSL
/// </summary>
//public class LdapUserServicesSSL : BaseLdapUserServices
//{
//    private static ILogger logger = Log.ForContext(typeof(LdapUserServices));

//    /// <summary>
//    ///
//    /// </summary>
//    /// <param name="info"></param>
//    public LdapUserServicesSSL(LdapConfig info)
//        : base(info)
//    { }

//    #region Public members

//    /// <summary>
//    /// Ricerca di oggetti di tipo "user" contenuti in un particolare gruppo di LDAP
//    /// </summary>
//    /// <returns></returns>
//    public override LdapUser[] GetUsers()
//    {

//        List<LdapUser> users = new List<LdapUser>();

//        using (LdapConnection con = this.CreateLdapConnection())
//        {

//            try
//            {
//                con.Bind();

//                SearchRequest request = new SearchRequest(this.Info.GroupDN, "(objectClass=*)", System.DirectoryServices.Protocols.SearchScope.Subtree);//"(objectClass=group)"

//                SearchResponse response = (SearchResponse)con.SendRequest(request);

//                foreach (SearchResultEntry entry in response.Entries)
//                {
//                    // L'attributo "member" contiene i DN di tutti gli utenti del gruppo
//                    DirectoryAttribute attribute = entry.Attributes["member"];

//                    for (int i = 0; i < attribute.Count; i++)
//                    {
//                        // Ogni occorrenza della collection "attribute" corrisponde ad un DN dell'utente appartenente al gruppo

//                        string dn = string.Empty;

//                        // Reperimento del distinguished name
//                        byte[] bytes = attribute[i] as byte[];

//                        if (bytes != null)
//                            // Presenza di caratteri che i directoryservices non riescono a descriptare (es. accentanti)
//                            dn = ASCIIEncoding.Default.GetString(bytes);
//                        else
//                            dn = attribute[i].ToString();

//                        // Ricerca singolo utente
//                        SearchRequest requestUser = new SearchRequest(dn,
//                                    "(objectClass=*)", //"(objectClass=person)",
//                                    System.DirectoryServices.Protocols.SearchScope.Subtree);

//                        SearchResponse responseUser = (SearchResponse)con.SendRequest(requestUser);

//                        foreach (SearchResultEntry entryUser in responseUser.Entries)
//                        {
//                            users.Add(this.CreateLdapUserInstance(entryUser));
//                        }
//                    }
//                }
//            }
//            catch (LdapException ex)

//            {
//                throw new ApplicationException("Percorso LDAPS o credenziali di dominio non valide: " + ex.Message + " LDAP connection info: " + "Count client certificates = " + con.ClientCertificates.Count + " Parametri usati per creare la connessione: user -> " + this.Info.DomainUserName + " pw -> " + this.Info.DomainUserPassword);

//            }
//            catch (DirectoryOperationException ex2)
//            {
//                throw new ApplicationException("Credenziali di dominio non valide: " + ex2.Message);
//            }
//        }

//        return users.ToArray();
//    }

//    /// <summary>
//    /// Autenticazione utente ad LDAP
//    /// </summary>
//    /// <param name="userName"></param>
//    /// <param name="password"></param>
//    /// <returns></returns>
//    public override bool AuthenticateUser(string userName, string password)
//    {
//        bool retValue = false;
//        //logger.DebugFormat("AuthenticateUser SSL -> uid {0}  pass {1}", userName, password);

//        using (LdapConnection con = this.CreateLdapConnection(userName, password))
//        {
//            try
//            {
//                con.Bind();

//                retValue = true;
//            }
//            catch (Exception ex)
//            {
//                logger.Debug("Percorso LDAP o credenziali di dominio non valide", ex);

//                retValue = false;
//            }
//        }

//        return retValue;
//    }

//    #endregion

//    #region Protected members

//    /// <summary>
//    /// Path del certificato utilizzato per autenticarsi al server LDAP via SSL
//    /// </summary>
//    protected string CertificatePath
//    {
//        get
//        {
//            const string CONFIG_KEY = "LdapUserSyncCertificatePath";
//            if (string.IsNullOrEmpty(DocsPaVO.Settings.AppSettings.Instance.LdapUserSyncCertificatePath))
//                throw new ApplicationException("Percorso del certificato per l'autenticazione ad LDAP non impostato");
//            return DocsPaVO.Settings.AppSettings.Instance.LdapUserSyncCertificatePath;
//        }
//    }


//    /// <summary>
//    ///
//    /// </summary>
//    /// <param name="entryUser"></param>
//    /// <param name="propertyName"></param>
//    /// <param name="nullable"></param>
//    /// <param name="defaultValue"></param>
//    /// <returns></returns>
//    protected string GetPropertyValue(SearchResultEntry entryUser, string propertyName, bool nullable, string defaultValue)
//    {
//        if (!nullable && !entryUser.Attributes.Contains(propertyName))
//            throw new ApplicationException(string.Format("Valore richiesto per la proprietà '{0}' dell'utente '{1}' in LDAP", propertyName, GetPropertyValue(entryUser, "distinguishedName", true, string.Empty)));
//        else
//        {
//            if (!entryUser.Attributes.Contains(propertyName))
//                return defaultValue;
//            else
//            {
//                byte[] bytes = entryUser.Attributes[propertyName][0] as byte[];

//                if (bytes != null)
//                    return Encoding.Default.GetString(bytes);
//                else
//                    return entryUser.Attributes[propertyName][0].ToString();
//            }
//        }
//    }

//    /// <summary>
//    /// Creazione di una nuova connessione ad ldap tramite le credenziali di un utente fornite come parametri
//    /// </summary>
//    /// <param name="userName"></param>
//    /// <param name="password"></param>
//    /// <returns></returns>
//    protected virtual LdapConnection CreateLdapConnection(string userName, string password)
//    {
//        logger.Debug("Inizio Connession tramite SSL uid: {0} pass {1}", userName, password);
//        LdapConnection con = new LdapConnection(new LdapDirectoryIdentifier(this.Info.Host));
//        con.SessionOptions.SecureSocketLayer = true;
//        con.SessionOptions.ProtocolVersion = 3;
//        con.SessionOptions.VerifyServerCertificate = new VerifyServerCertificateCallback(this.OnServerAuthenticate);
//        con.Credential = new NetworkCredential(userName, password);
//        con.AuthType = AuthType.Basic;
//        return con;
//    }

//    /// <summary>
//    /// Creazione di una nuova connessione ad ldap
//    /// </summary>
//    /// <returns></returns>
//    protected virtual LdapConnection CreateLdapConnection()
//    {
//        return this.CreateLdapConnection(this.Info.DomainUserName, this.Info.DomainUserPassword);
//    }

//    /// <summary>
//    /// Creazione di un'istanza dell'utente in LDAP contenente gli attributi significativi richiesti
//    /// </summary>
//    /// <param name="entryUser"></param>
//    /// <returns></returns>
//    protected LdapUser CreateLdapUserInstance(SearchResultEntry entryUser)
//    {
//        LdapUser ldapUser = new LdapUser();

//        try
//        {
//            //ldapUser.DN = this.GetPropertyValue(entryUser, "distinguishedName", false, string.Empty);
//            ldapUser.DN = entryUser.DistinguishedName;
//            ldapUser.UserID = this.GetPropertyValue(entryUser, this.Info.UserAttributes.UserId, false, string.Empty);
//            ldapUser.Matricola = this.GetPropertyValue(entryUser, this.Info.UserAttributes.Matricola, false, string.Empty);
//            ldapUser.Email = this.GetPropertyValue(entryUser, this.Info.UserAttributes.Email, false, string.Empty);
//            ldapUser.Nome = this.GetPropertyValue(entryUser, this.Info.UserAttributes.Nome, false, string.Empty);
//            ldapUser.Cognome = this.GetPropertyValue(entryUser, this.Info.UserAttributes.Cognome, false, string.Empty);
//            ldapUser.Sede = this.GetPropertyValue(entryUser, this.Info.UserAttributes.Sede, true, string.Empty);
//        }
//        catch (Exception ex)
//        {
//            logger.Debug(ex.Message);

//            ldapUser.ErrorMessage = ex.Message;
//        }

//        return ldapUser;
//    }

//    /// <summary>
//    /// Handler per l'evento VerifyServerCertificateCallback per la verifica
//    /// delle credenziali per l'accesso al server LDAP tramite certificato
//    /// </summary>
//    /// <param name="connection"></param>
//    /// <param name="certificate"></param>
//    /// <returns></returns>
//    protected virtual bool OnServerAuthenticate(LdapConnection connection, X509Certificate certificate)
//    {

//        string certPath = this.CertificatePath;
//        if ((certPath.ToLower() == "false") ||
//           (certPath.ToLower() == "0"))
//            return true;

//        try
//        {
//            // Reperimento del certificato configurato
//            X509Certificate expectedCert = X509Certificate.CreateFromCertFile(this.CertificatePath);

//            if (expectedCert.Equals(certificate))
//            {
//                return true;
//            }
//            else
//            {
//                // certificate.ToString(true) provides verbose information about the certificate

//                string errorMessage =
//                    String.Format(
//                    "Il certificato non corrisponde con quello restituito dal server: {0}",
//                    certificate.ToString(true));

//                logger.Debug(errorMessage);

//                return false;
//            }
//        }
//        catch (Exception ex)
//        {
//            logger.Debug("Cannot validate certificate: " + ex.Message);
//            return false;
//        }
//    }

//    #endregion
//}

public class LdapUserServicesSSL : BaseLdapUserServices
{
    private static Serilog.ILogger logger = Log.ForContext(typeof(LdapUserServicesSSL));

    public LdapUserServicesSSL(LdapConfig info) : base(info)
    {
    }

    public override bool AuthenticateUser(string userName, string password)
    {
        bool retValue = false;
        //logger.DebugFormat("AuthenticateUser SSL -> uid {0}  pass {1}", userName, password);

        using (LdapConnection con = CreateLdapConnection())
        {
            try
            {
                con.Bind(userName, password);

                retValue = con.Bound;
            }
            catch (Exception ex)
            {
                logger.Debug("Percorso LDAP o credenziali di dominio non valide", ex);

                retValue = false;
            }
        }

        return retValue;
    }

    public override LdapUser[] GetUsers()
    {

        List<LdapUser> users = new List<LdapUser>();

        using (LdapConnection con = this.CreateLdapConnection())
        {

            try
            {
                con.Bind(this.Info.DomainUserName, this.Info.DomainUserPassword);

                if (con.Bound)
                {
                    logger.Information($"User {this.Info.DomainUserName} LDAP bind successfull (Password verification passed).");
                }

                //SearchRequest request = new SearchRequest(this.Info.GroupDN, "(objectClass=*)", System.DirectoryServices.Protocols.SearchScope.Subtree);//"(objectClass=group)"
                //SearchResponse response = (SearchResponse)con.SendRequest(request);
                var searchFilter = this.Info.GroupDN.Replace("(", "\\28").Replace(")", "\\29");

                con.SearchConstraints.ReferralFollowing = true;
                con.SearchConstraints.MaxResults = 1000;

                var searchResults = con.Search(
                    searchFilter,
                    LdapConnection.ScopeSub,
                    "(objectClass=groupOfNames)",
                    new[] { "cn", "member" }, // Aggiungi gli attributi che desideri recuperare
                    false
                );
                logger.Information($"Search for {this.Info.GroupDN} completed.");

                while (searchResults.HasMore())
                {
                    logger.Information($"found record");

                    var entry = searchResults.Next();

                    // L'attributo "member" contiene i DN di tutti gli utenti del gruppo
                    var members = entry.GetAttribute("member")?.StringValues;
                    if (members != null)
                    {
                        while (members.MoveNext())
                        {
                            // Ogni occorrenza della collection "attribute" corrisponde ad un DN dell'utente appartenente al gruppo
                            logger.Information($"Member: {members.Current}");

                            // Perform a separate search for each member to retrieve their attributes
                            var memberSearchResults = con.Search(
                                members.Current,
                                LdapConnection.ScopeBase,
                                "(objectClass=*)", // Retrieve all attributes
                                null,
                                false
                            );

                            while (memberSearchResults.HasMore())
                            {
                                var memberEntry = memberSearchResults.Next();
                                //var memberAttributeSet = memberEntry.GetAttributeSet();

                                users.Add(this.CreateLdapUserInstance(memberEntry));
                            }
                        }
                    }
                }
            }
            catch (LdapException ex)
            {
                if (ex.ResultCode == LdapException.ServerDown || ex.ResultCode == LdapException.ConnectError)
                {
                    logger.Error($"Error during LDAP connect/bind! User={this.Info.DomainUserName}, Server={this.Info.Host}, Error={ex.Message}");
                    throw;
                }
                else if (ex.ResultCode == LdapException.InvalidCredentials)
                {
                    logger.Error($"Username {this.Info.DomainUserName} and password don't match!");
                    throw;
                }
                else
                {
                    logger.Error($"Error during LDAP connect/bind! User={this.Info.DomainUserName}, Server={this.Info.Host}, Error={ex.Message}");
                    throw;
                }
            }
            catch (Exception ex)
            {
                logger.Fatal(ex, $"Errore: {ex.Message}");

                throw;
            }
        }

        return users.ToArray();
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="entryUser"></param>
    /// <param name="propertyName"></param>
    /// <param name="nullable"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    protected string GetPropertyValue(LdapEntry entryUser, string propertyName, bool nullable, string defaultValue)
    {
        LdapAttribute attrEntryUsr = null;
        try
        {
            attrEntryUsr = entryUser.GetAttribute(propertyName);
        }
        catch (Exception)
        {
        }

        if (!nullable && attrEntryUsr == null)
            throw new ApplicationException(string.Format("Valore richiesto per la proprietà '{0}' dell'utente '{1}' in LDAP", propertyName, GetPropertyValue(entryUser, this.Info.UserAttributes.Matricola, true, string.Empty)));
        else
        {
            if (attrEntryUsr == null)
                return defaultValue;
            else
            {
                byte[] bytes = attrEntryUsr.ByteValue;

                if (bytes != null)
                    return Encoding.Default.GetString(bytes);
                else
                    return attrEntryUsr.StringValue;
            }
        }
    }

    /// <summary>
    /// Creazione di una nuova connessione ad ldap tramite le credenziali di un utente fornite come parametri
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    protected virtual LdapConnection CreateLdapConnection()
    {
        //logger.Debug("Inizio Connession tramite SSL uid: {0} pass {1}", userName, password);
        //LdapConnection con = new LdapConnection(new LdapDirectoryIdentifier(this.Info.Host));
        //con.SessionOptions.SecureSocketLayer = true;
        //con.SessionOptions.ProtocolVersion = 3;
        //con.SessionOptions.VerifyServerCertificate = new VerifyServerCertificateCallback(this.OnServerAuthenticate);
        //con.Credential = new NetworkCredential(userName, password);
        //con.AuthType = AuthType.Basic;
        var conn = new LdapConnection() { SecureSocketLayer = true, ConnectionTimeout = 1000 };
        conn.SecureSocketLayer = true;
        conn.UserDefinedServerCertValidationDelegate += new
            Novell.Directory.Ldap.RemoteCertificateValidationCallback(CheckCertificate);

        var host = this.Info.Host;
        host = host.Replace("ldaps://", "ldap://");
        host = host.Replace(":636", "");

        conn.Connect(host, LdapConnection.DefaultSslPort);
        conn.ConnectionTimeout = 10000; // Timeout di 10 secondi
        return conn;
    }

    /// <summary>
    /// Creazione di un'istanza dell'utente in LDAP contenente gli attributi significativi richiesti
    /// </summary>
    /// <param name="entryUser"></param>
    /// <returns></returns>
    protected LdapUser CreateLdapUserInstance(LdapEntry entryUser)
    {
        LdapUser ldapUser = new LdapUser();

        try
        {
            //ldapUser.DN = this.GetPropertyValue(entryUser, "distinguishedName", false, string.Empty);
            ldapUser.DN = entryUser.Dn;
            ldapUser.UserID = this.GetPropertyValue(entryUser, this.Info.UserAttributes.UserId, false, string.Empty);
            ldapUser.Matricola = this.GetPropertyValue(entryUser, this.Info.UserAttributes.Matricola, false, string.Empty);
            ldapUser.Email = this.GetPropertyValue(entryUser, this.Info.UserAttributes.Email, false, string.Empty);
            ldapUser.Nome = this.GetPropertyValue(entryUser, this.Info.UserAttributes.Nome, false, string.Empty);
            ldapUser.Cognome = this.GetPropertyValue(entryUser, this.Info.UserAttributes.Cognome, false, string.Empty);
            ldapUser.Sede = this.GetPropertyValue(entryUser, this.Info.UserAttributes.Sede, true, string.Empty);
        }
        catch (Exception ex)
        {
            logger.Debug(ex.Message);

            ldapUser.ErrorMessage = ex.Message;
        }

        return ldapUser;
    }

    public bool CheckCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) => true;
}
