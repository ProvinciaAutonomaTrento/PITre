// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace DocsPaVO.Ldap
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    [DataContract]
    public class LdapConfig
    {
        /// <summary>
        /// 
        /// </summary>
        private string _serverName = string.Empty;

        /// <summary>
        /// Indica se l'integrazione ldap è attiva o meno
        /// </summary>
        [DataMember]
        public bool LdapIntegrationActive
        {
            get;
            set;
        }

        /// <summary>
        /// Nome del server LDAP
        /// </summary>
        [DataMember]
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
        [DataMember]
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
        [DataMember]
        public string DomainUserName
        {
            get;
            set;
        }

        /// <summary>
        /// Password per l'utente di dominio per l'autenticazione ad LDAP
        /// </summary>
        [DataMember]
        public string DomainUserPassword
        {
            get;
            set;
        }

        /// <summary>
        /// Distinguished name del gruppo in LDAP che si intende analizzare nella sincronizzazione utenti
        /// </summary>
        [DataMember]
        public string GroupDN
        {
            get;
            set;
        }

        /// <summary>
        /// Attributi ldap per utente docspa
        /// </summary>
        [DataMember]
        public LdapUserAttributes UserAttributes
        {
            get;
            set;
        }

        /// <summary>
        /// Indica di utilizzare il protocollo SSL per la connessione ad LDAP
        /// </summary>
        [DataMember]
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
    [DataContract]
    public class LdapUserAttributes
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string UserId
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Matricola
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Email
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Nome
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Cognome
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Sede
        {
            get;
            set;
        }
    }
}
