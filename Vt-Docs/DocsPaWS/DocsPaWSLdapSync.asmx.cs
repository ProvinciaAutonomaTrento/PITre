using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;

namespace DocsPaWS
{
    /// <summary>
    /// Servizi per la sincronizzazione con LDAP
    /// </summary>
    [WebService(Namespace = "http://valueteam.com/DocsPa/LdapSync")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class DocsPaWSLdapSync : System.Web.Services.WebService
    {
        /// <summary>
        /// Sincronizzazione degli utenti dell'amministrazione con LDAP
        /// </summary>
        /// <param name="userId">Nome utente amministratore che effettua la sincronizzazione</param>
        /// <param name="password">Password utente amministratore che effettua la sincronizzazione</param>
        /// <param name="adminCode">Codice amministrazione da sincronizzare</param>
        /// <returns></returns>
        [WebMethod(Description  = "Sincronizzazione degli utenti dell'amministrazione con LDAP")]
        public DocsPaVO.Ldap.LdapSyncronizationResponse SyncronizeLdapUsers(string userId, string password, string adminCode)
        {
            return BusinessLogic.Ldap.LdapUserSyncronizationServices.SyncronizeLdapUsers(userId, password, adminCode);
        }
    }
}
