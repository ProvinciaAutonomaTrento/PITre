using System;
using System.Collections.Generic;
using System.Text;

namespace DocsPaVO.Ldap
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class LdapConfig
    {
        /// <summary>
        /// 
        /// </summary>
        private string _serverName = string.Empty;

        /// <summary>
        /// Indica se l'integrazione ldap è attiva o meno
        /// </summary>
        public bool LdapIntegrationActive
        {
            get;
            set;
        }

        /// <summary>
        /// Nome del server LDAP
        /// </summary>
        public string ServerName
        {
            get
            {
                if (!string.IsNullOrEmpty(this._serverName))
                    return new Uri(this._serverName).ToString();
                else
                    return string.Empty;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    this._serverName = new Uri(value).ToString();
                else
                    this._serverName = string.Empty;
            }
        }

        /// <summary>
        /// Reperimento della sola informazione relativa all'host e alla porta
        /// </summary>
        public string Host
        {
            get
            {
                if (!string.IsNullOrEmpty(this.ServerName))
                    return new Uri(this.ServerName).Authority;
                else
                    return string.Empty;
            }
        }

        /// <summary>
        /// Nome utente di dominio per l'autenticazione ad LDAP
        /// </summary>
        public string DomainUserName
        {
            get;
            set;
        }

        /// <summary>
        /// Password per l'utente di dominio per l'autenticazione ad LDAP
        /// </summary>
        public string DomainUserPassword
        {
            get;
            set;
        }

        /// <summary>
        /// Distinguished name del gruppo in LDAP che si intende analizzare nella sincronizzazione utenti
        /// </summary>
        public string GroupDN
        {
            get;
            set;
        }

        /// <summary>
        /// Attributi ldap per utente docspa
        /// </summary>
        public LdapUserAttributes UserAttributes
        {
            get;
            set;
        }

        /// <summary>
        /// Indica di utilizzare il protocollo SSL per la connessione ad LDAP
        /// </summary>
        public bool SSL
        {
            get
            {
                if (!string.IsNullOrEmpty(this.ServerName))
                    return new Uri(this.ServerName).Scheme.Equals("ldaps");
                else
                    return false;
            }
        }
    }

    /// <summary>
    /// Classe per la gestione del mapping degli attributi docspa con i corrispondenti attributi degli utenti ldap
    /// </summary>
    [Serializable()]
    public class LdapUserAttributes
    {
        /// <summary>
        /// 
        /// </summary>
        public string UserId
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Matricola
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Email
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Nome
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Cognome
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Sede
        {
            get;
            set;
        }
    }
}
