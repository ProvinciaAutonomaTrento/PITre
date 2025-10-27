// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
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
    public class LdapSyncronizationHistoryItem
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// Utente che ha effettuato la sincronizzazione
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// Data sincronizzazione
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Numero di elementi sincronizzati
        /// </summary>
        public int ItemsSyncronized { get; set; }

        /// <summary>
        /// Errore di sincronizzazione
        /// </summary>
        public string ErrorDetails { get; set; }
    }

    /// <summary>
    /// Dati della richiesta della sincronizzazione di un utente ldap in docspa
    /// </summary>
    [Serializable()]
    public class LdapSyncronizationRequest
    {
        /// <summary>
        /// 
        /// </summary>
        public DocsPaVO.amministrazione.InfoUtenteAmministratore InfoUtente;

        /// <summary>
        /// 
        /// </summary>
        public string IdAmministrazione;
    }

    /// <summary>
    /// Mantiene i dettagli sull'esito della sincronizzazione di un utente ldap in docspa
    /// </summary>
    [Serializable()]
    [DataContract]
    public class LdapSyncronizationResponse
    {
        /// <summary>
        /// 
        /// </summary>
        public LdapSyncronizationResponse()
        {
            this.Date = DateTime.Now;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="adminCode"></param>
        public LdapSyncronizationResponse(string user, string adminCode)
            : this()
        {
            this.User = user;
            this.AdminCode = adminCode;
        }

        /// <summary>
        /// Utente che ha effettuato la sincronizzazione
        /// </summary>
        [DataMember]
        public string User { get; set; }

        /// <summary>
        /// Codice dell'amministrazione oggetto di sincronizzazione
        /// </summary>
        [DataMember]
        public string AdminCode { get; set; }

        /// <summary>
        /// Data sincronizzazione
        /// </summary>
        [DataMember]
        public DateTime Date { get; set; }

        /// <summary>
        /// Elementi di sincronizzazione
        /// </summary>
        [DataMember]
        public LdapSyncronizationResponseItem[] Items { get; set; } = new LdapSyncronizationResponseItem[0];

        /// <summary>
        /// Numero di elementi sincronizzati
        /// </summary>
        [DataMember]
        public int ItemsSyncronized { get; set; }

        /// <summary>
        /// Errore di sincronizzazione
        /// </summary>
        [DataMember]
        public string ErrorDetails { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    [DataContract]
    public class LdapSyncronizationResponseItem
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string UserId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public DateTime Date { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public LdapSyncronizationResultEnum Result { get; set; } = LdapSyncronizationResultEnum.Error;

        /// <summary>
        /// Dettagli di sincronizzazione
        /// </summary>
        [DataMember]
        public string Details { get; set; }
    }

    /// <summary>
    /// Tipologie di sincronizzazione utente
    /// </summary>
    public enum LdapSyncronizationResultEnum
    {
        Error,
        Inserted,
        Updated,
        Deleted
    }
}