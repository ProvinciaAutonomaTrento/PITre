using System;
using System.Collections.Generic;
using System.Text;

namespace DocsPaVO.Ldap
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class LdapSyncronizationHistoryItem
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id;

        /// <summary>
        /// Utente che ha effettuato la sincronizzazione
        /// </summary>
        public string User;

        /// <summary>
        /// Data sincronizzazione
        /// </summary>
        public DateTime Date;

        /// <summary>
        /// Numero di elementi sincronizzati
        /// </summary>
        public int ItemsSyncronized;

        /// <summary>
        /// Errore di sincronizzazione
        /// </summary>
        public string ErrorDetails;
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
        public DocsPaVO.utente.InfoUtente InfoUtente;

        /// <summary>
        /// 
        /// </summary>
        public string IdAmministrazione;
    }

    /// <summary>
    /// Mantiene i dettagli sull'esito della sincronizzazione di un utente ldap in docspa
    /// </summary>
    [Serializable()]
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
        public string User;

        /// <summary>
        /// Codice dell'amministrazione oggetto di sincronizzazione
        /// </summary>
        public string AdminCode;

        /// <summary>
        /// Data sincronizzazione
        /// </summary>
        public DateTime Date;

        /// <summary>
        /// Elementi di sincronizzazione
        /// </summary>
        public LdapSyncronizationResponseItem[] Items = new LdapSyncronizationResponseItem[0];

        /// <summary>
        /// Numero di elementi sincronizzati
        /// </summary>
        public int ItemsSyncronized;

        /// <summary>
        /// Errore di sincronizzazione
        /// </summary>
        public string ErrorDetails;
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class LdapSyncronizationResponseItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string UserId;

        /// <summary>
        /// 
        /// </summary>
        public DateTime Date;

        /// <summary>
        /// 
        /// </summary>
        public LdapSyncronizationResultEnum Result = LdapSyncronizationResultEnum.Error;

        /// <summary>
        /// Dettagli di sincronizzazione
        /// </summary>
        public string Details;
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