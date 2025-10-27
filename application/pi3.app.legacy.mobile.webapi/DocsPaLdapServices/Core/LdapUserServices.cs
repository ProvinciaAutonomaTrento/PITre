// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.Ldap;
using Serilog;
#if false
using System.DirectoryServices;

#endif
namespace DocsPaLdapServices.Core;

/// <summary>
/// Classe che espone dei servizi per la gestione di oggetti 'user' in LDAP
/// </summary>
public class LdapUserServices : BaseLdapUserServices
{
    private static ILogger logger = Log.ForContext(typeof(LdapUserServices));
    /// <summary>
    /// 
    /// </summary>
    /// <param name="info"></param>
    public LdapUserServices(LdapConfig info)
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

#if false // richiede directory services
        DirectoryEntry container = CreateDirectoryEntry(this.Info.GroupDN);

        if (container.SchemaClassName == OBJECT_TYPE_GROUP)
        {
            foreach (string member in container.Properties["member"])
            {
                DirectoryEntry memberEntry = CreateDirectoryEntry(member);

                // Verifica se l'oggeto richiesto è stato trovato e se la classe dell'oggetto è "user"
                if (memberEntry.SchemaClassName == OBJECT_TYPE_USER)
                {
                    users.Add(this.CreateLdapUserInstance(memberEntry));
                }
            }
        }

#endif
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
        //logger.DebugFormat("AuthenticateUser SSL -> uid {0}  pass {1}", userName, password);
#if false
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
#endif

        return retValue;
    }




#if false // richiede directory service
    /// <summary>
    /// 
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public override bool AuthenticateUser(string userName, string password)
    {
        bool retValue = false;

        DirectoryEntry entry = new DirectoryEntry
        {
            Path = string.Concat(this.Info.ServerName.ToUpper(), this.Info.GroupDN),
            Username = userName,
            Password = password
        };

        //logger.Debug(string.Format("AuthenticateUser - Server: {0}, DN: {1}, UserName: {2}",
        this.Info.ServerName, this.Info.GroupDN, userName));

        if (!this.CheckDirectoryEntry(entry))
        {
            //logger.Debug("Percorso LDAP o credenziali di dominio non valide");

            retValue = false;
        }
        else
        {
            retValue = true;
        }

        return retValue;
    }
#endif

    #endregion

    #region Protected members

#if false // richiede directory service
    /// <summary>
    /// Creazione di un'istanza dell'utente in LDAP contenente gli attributi significativi richiesti
    /// </summary>
    /// <param name="member"></param>
    /// <returns></returns>
    protected LdapUser CreateLdapUserInstance(DirectoryEntry member)
    {
        LdapUser ldapUser = new LdapUser();

        try
        {
            ldapUser.DN = GetPropertyValue<string>(member, "distinguishedName", false, string.Empty);
            ldapUser.UserID = GetPropertyValue<string>(member, this.Info.UserAttributes.UserId, false, string.Empty);
            ldapUser.Matricola = GetPropertyValue<string>(member, this.Info.UserAttributes.Matricola, false, string.Empty);
            ldapUser.Email = GetPropertyValue<string>(member, this.Info.UserAttributes.Email, false, string.Empty);
            ldapUser.Nome = GetPropertyValue<string>(member, this.Info.UserAttributes.Nome, false, string.Empty);
            ldapUser.Cognome = GetPropertyValue<string>(member, this.Info.UserAttributes.Cognome, false, string.Empty);
            ldapUser.Sede = GetPropertyValue<string>(member, this.Info.UserAttributes.Sede, true, string.Empty);
        }
        catch (Exception ex)
        {
            //logger.Debug(ex);

            ldapUser.ErrorMessage = ex.Message;
        }

        return ldapUser;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="member"></param>
    /// <param name="propertyName"></param>
    /// <param name="nullable"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    protected T GetPropertyValue<T>(DirectoryEntry member, string propertyName, bool nullable, T defaultValue)
    {
        if (!nullable && member.Properties[propertyName].Value == null)
            throw new ApplicationException(string.Format("Valore richiesto per la proprietà '{0}' dell'utente '{1}' in LDAP", propertyName, GetPropertyValue<string>(member, "distinguishedName", true, string.Empty)));
        else
        {
            if (member.Properties[propertyName].Value == null)
                return defaultValue;
            else
                return (T)member.Properties[propertyName].Value;
        }
    } 
#endif

    #endregion

    #region Private members

#if false // richiede directory service

    /// <summary>
    /// 
    /// </summary>
    /// <param name="distinguishedName"></param>
    /// <returns></returns>
    private DirectoryEntry CreateDirectoryEntry(string distinguishedName)
    {
        // Prova per evitare il disposed object.
        //using (DirectoryEntry entry = new DirectoryEntry())
        //{
        DirectoryEntry entry = new DirectoryEntry();
        entry.Path = string.Concat(this.Info.ServerName.ToUpper(), distinguishedName);

        if (!string.IsNullOrEmpty(this.Info.DomainUserName) && !string.IsNullOrEmpty(this.Info.DomainUserPassword))
        {
            entry.Username = this.Info.DomainUserName;
            entry.Password = this.Info.DomainUserPassword;
        }

        logger.Debug(string.Format("CreateDirectoryEntry - Server: {0}, DN: {1}, UserName: {2}",
                                                        this.Info.ServerName, distinguishedName, this.Info.DomainUserName));

        if (!this.CheckDirectoryEntry(entry))
            throw new ApplicationException("Percorso LDAP o credenziali di dominio non valide");

        return entry;
        //}
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="entry"></param>
    private bool CheckDirectoryEntry(DirectoryEntry entry)
    {
        try
        {
            string schema = entry.SchemaClassName;

            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
#endif

    #endregion
}
