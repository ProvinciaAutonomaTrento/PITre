using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Ldap
{
    /// <summary>
    /// Configurazioni LDAP applicate ad utente in amministrazione
    /// </summary>
    [Serializable()]
    public class LdapUserConfig
    {
        /// <summary>
        /// Id del corrispondente utente in LDAP
        /// </summary>
        public string LdapIdSync
        {
            get;
            set;
        }

        /// <summary>
        /// Determina se l'utente è sincronizzato con i dati del corrispondente utente in LDAP
        /// </summary>
        public bool LdapSyncronized
        {
            get;
            set;
        }

        /// <summary>
        /// Determina se l'utente è autenticato con il corrispondente utente in LDAP
        /// </summary>
        public bool LdapAuthenticated
        {
            get;
            set;
        }

        ///// <summary>
        ///// Configurazioni per la connessione ad LDAP per l'utente
        ///// </summary>
        //public LdapConfig LdapUserConfigSettings
        //{
        //    get;
        //    set;
        //}
    }
}
