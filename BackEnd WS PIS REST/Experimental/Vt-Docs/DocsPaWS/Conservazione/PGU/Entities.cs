using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaWS.Conservazione.PGU
{
    #region PguService
    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class PguServiceRequest
    {
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class PguServiceResponse
    {
        /// <summary>
        /// 
        /// </summary>
        public bool Success
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Exception
        {
            get;
            set;
        }
    }

    #endregion

    #region GetToken
    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class GetTokenRequest : PguServiceRequest
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
        public string Ente
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class GetTokenResponse : PguServiceResponse
    {
        /// <summary>
        /// 
        /// </summary>
        public string Token
        {
            get;
            set;
        }
    }

    #endregion

    #region GetCountIstanzeConservazione
    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class GetCountIstanzeConservazioneRequest : PguServiceRequest
    {
        /// <summary>
        /// 
        /// </summary>
        public string CodiceAmministrazione
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class GetCountIstanzeConservazioneResponse : PguServiceResponse
    {
        /// <summary>
        /// Numero totale di istanze da inviare
        /// </summary>
        public int DaInviare
        {
            get;
            set;
        }

        /// <summary>
        /// Numero totale di istanze inviate
        /// </summary>
        public int Inviate
        {
            get;
            set;
        }

        /// <summary>
        /// Numero totale di istanze rifiutate
        /// </summary>
        public int Rifiutate
        {
            get;
            set;
        }

        /// <summary>
        /// Numero totale di istanze in lavorazione
        /// </summary>
        public int InLavorazione
        {
            get;
            set;
        }

        /// <summary>
        /// Numero totale di istanze firmate
        /// </summary>
        public int Firmate
        {
            get;
            set;
        }

        /// <summary>
        /// Indica il numero totale di istanza in stato conservate
        /// </summary>
        public int Conservate
        {
            get;
            set;
        }

        /// <summary>
        /// Numero totale di istanze chiuse
        /// </summary>
        public int Chiuse
        {
            get;
            set;
        }

        /// <summary>
        /// Numero totale di notifiche pending
        /// </summary>
        public int Notifiche
        {
            get;
            set;
        }
    }
    #endregion

    #region GetCountIstanzeEsibizione
    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class GetCountIstanzeEsibizioneRequest : PguServiceRequest
    {
        /// <summary>
        /// 
        /// </summary>
        public string CodiceAmministrazione
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class GetCountIstanzeEsibizioneResponse : PguServiceResponse
    {
        /// <summary>
        /// Numero totale di istanze di esibizione da Certificare
        /// </summary>
        public int DaCertificare
        {
            get;
            set;
        }
    }
    #endregion

    #region GetUser
    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class GetUserRequest : PguServiceRequest
    {
        /// <summary>
        /// 
        /// </summary>
        public string UserCode
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Ente
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class GetUserResponse : PguServiceResponse
    {
        /// <summary>
        /// Indica la presenza dell'utente nella people
        /// </summary>
        public bool UtentePresente
        {
            get;
            set;
        }

        /// <summary>
        /// Indica se l'utente è abilitato al centro servizi
        /// </summary>
        public bool AbilitatoCentroServizi
        {
            get;
            set;
        }
    }
    #endregion

    #region InsertUser
    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class InsertUserRequest : PguServiceRequest
    {
        /// <summary>
        /// Codice Utente
        /// </summary>
        public string UserCode
        {
            get;
            set;
        }

        /// <summary>
        /// Codice Ente (Codice Amministrazione)
        /// </summary>
        public string Ente
        {
            get;
            set;
        }

        /// <summary>
        /// Email utente
        /// </summary>
        public string Email
        {
            get;
            set;
        }

        /// <summary>
        /// Nome utente
        /// </summary>
        public string Nome
        {
            get;
            set;
        }

        /// <summary>
        /// Cognome utente
        /// </summary>
        public string Cognome
        {
            get;
            set;
        }

        /// <summary>
        /// Password utente
        /// </summary>
        public string Password
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class InsertUserResponse : PguServiceResponse
    {
        /// <summary>
        /// Indica se l'utente è abilitato al centro servizi
        /// </summary>
        public bool EsitoInserimento
        {
            get;
            set;
        }
    }
    #endregion

    #region UpdateUser
    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class UpdateUserRequest : PguServiceRequest
    {
        /// <summary>
        /// 
        /// </summary>
        public string UserCode
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Ente
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class UpdateUserResponse : PguServiceResponse
    {
        /// <summary>
        /// Indica se l'utente è abilitato al centro servizi
        /// </summary>
        public bool EsitoAggiornamento
        {
            get;
            set;
        }
    }
    #endregion

    #region GetStampaRegistro
    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class GetStampaRegistroRequest : PguServiceRequest
    {
        /// <summary>
        /// 
        /// </summary>
        public string idAmm
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class GetStampaRegistroResponse : PguServiceResponse
    {
        /// <summary>
        /// La codifica è la seguente:
        /// - 10: Giornaliera
        /// - 20: Settimanale
        /// - 30: Mensile
        /// - 40: Annuale
        /// </summary>
        public string FrequenzaStampaRegistro
        {
            get;
            set;
        }

        /// <summary>
        /// Valore del campo Abilitato per l'attivazione della stampa registri
        /// Il campo nella tabella DPA_CONFIG_STAMPA_CONS è DISABLED = 0 oppure 1
        /// </summary>
        public bool Abilitato
        {
            get;
            set;
        }

        /// <summary>
        /// Ora di stampa del registro
        /// </summary>
        public string OraStampa
        {
            get;
            set;
        }
    }

    #endregion

    #region SaveStampaRegistro
    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class SaveStampaRegistroRequest : PguServiceRequest
    {
        /// <summary>
        /// 
        /// </summary>
        public int idAmm
        {
            get;
            set;
        }

        /// <summary>
        /// Valore DISABLED della tabella DPA_CONFIG_STAMPA_CONS
        /// </summary>
        public int disabled
        {
            get;
            set;
        }

        /// <summary>
        /// Valore PRINT_FREQ della tabella DPA_CONFIG_STAMPA_CONS
        /// </summary>
        public int print_freq
        {
            get;
            set;
        }

        //Gabriele Melini 24-09-2013
        /// <summary>
        /// Valore PRINT_HOUR della tabella DPA_CONFIG_STAMPA_CONS
        /// </summary>
        public int print_hour
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class SaveStampaRegistroResponse : PguServiceResponse
    {
    }
    #endregion

    #region UpdateStampaRegistro
    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class UpdateStampaRegistroRequest : PguServiceRequest
    {
        /// <summary>
        /// 
        /// </summary>
        public int idAmm
        {
            get;
            set;
        }

        /// <summary>
        /// Valore DISABLED della tabella DPA_CONFIG_STAMPA_CONS
        /// </summary>
        public int disabled
        {
            get;
            set;
        }

        /// <summary>
        /// Valore PRINT_FREQ della tabella DPA_CONFIG_STAMPA_CONS
        /// </summary>
        public int print_freq
        {
            get;
            set;
        }

        /// <summary>
        /// GM 24-09-2013
        /// Valore PRINT_HOUR della tabella DPA_CONFIG_STAMPA_CONS
        /// </summary>
        public int print_hour
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class UpdateStampaRegistroResponse : PguServiceResponse
    {
    }
    #endregion

    #region Verifiche Periodiche

    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class GetVerifichePeriodicheRequest : PguServiceRequest
    {
        /// <summary>
        /// 
        /// </summary>
        public string idAmm
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class GetVerifichePeriodicheResponse : PguServiceResponse
    {
        /// <summary>
        /// 
        /// </summary>
        public string ScadenzaIntegrita
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string GiorniNotificheIntegrita
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string ScadenzaLeggibilita
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string GiorniNotificheLeggibilita
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class SaveVerifichePeriodicheRequest : PguServiceRequest
    {
        /// <summary>
        /// 
        /// </summary>
        public int idAmm
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string ScadenzaIntegrita
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string GiorniNotificheIntegrita
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string ScadenzaLeggibilita
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string GiorniNotificheLeggibilita
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class SaveVerifichePeriodicheResponse : PguServiceResponse
    {

    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class UpdateVerifichePeriodicheRequest : PguServiceRequest
    {
        /// <summary>
        /// 
        /// </summary>
        public int idAmm
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string ScadenzaIntegrita
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string GiorniNotificheIntegrita
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string ScadenzaLeggibilita
        {
            get;
            set;
        }

        public string GiorniNotificheLeggibilita
        {
            get;
            set;
        }

    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class UpdateVerifichePeriodicheResponse : PguServiceResponse
    {

    }

    #endregion

    #region Gestione Alert

    #region GET

    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class GetGestioneAlertRequest : PguServiceRequest
    {
        /// <summary>
        /// 
        /// </summary>
        public string idAmm
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class GetGestioneAlertResponse : PguServiceResponse
    {

        #region ALERT
        /// <summary>
        /// Attivazione alert esecuzione anticipata verifica leggiblità
        /// </summary>
        public bool abilitatoVerificaAnt
        {
            get;
            set;
        }

        /// <summary>
        /// Attivazione alert esecuzione verifica leggibilità su campione non consentito
        /// </summary>
        public bool abilitatoVerificaPerc
        {
            get;
            set;
        }

        /// <summary>
        /// Attivazione alert frequenza utilizzo verifica leggibilità singolo doc
        /// </summary>
        public bool abilitatoVerificaDoc
        {
            get;
            set;
        }

        /// <summary>
        /// Attivazione alert frequenza utilizzo download istanza
        /// </summary>
        public bool abilitatoDownload
        {
            get;
            set;
        }

        /// <summary>
        /// Attivazione alert frequenza utilizzo sfoglia istanza
        /// </summary>
        public bool abilitatoSfoglia
        {
            get;
            set;
        }

        /// <summary>
        /// Campione massimo doc verificabili in una verifica di leggibilità
        /// </summary>
        public string maxVerificaPerc
        {
            get;
            set;
        }

        /// <summary>
        /// Numero massimo operazioni verifica leggibilità singolo doc
        /// </summary>
        public string maxOperVerificaDoc
        {
            get;
            set;
        }

        /// <summary>
        /// Periodo monitoraggio verifica leggibilità singolo doc
        /// </summary>
        public string periodoMonVerificaDoc
        {
            get;
            set;
        }

        /// <summary>
        /// Numero massimo operazioni download istanza
        /// </summary>
        public string maxOperDownload
        {
            get;
            set;
        }

        /// <summary>
        /// Periodo monitoraggio download istanza
        /// </summary>
        public string periodoMonDownload
        {
            get;
            set;
        }

        /// <summary>
        /// Numero massimo operazioni sfoglia istanza
        /// </summary>
        public string maxOperSfoglia
        {
            get;
            set;
        }

        /// <summary>
        /// Periodo monitoraggio sfoglia istanza
        /// </summary>
        public string periodoMonSfoglia
        {
            get;
            set;
        }
        #endregion

        #region configurazione mail

        /// <summary>
        /// Indirizzo server SMTP
        /// </summary>
        public string serverSMTP
        {
            get;
            set;
        }

        /// <summary>
        /// Porta server SMTP
        /// </summary>
        public string portaSMTP
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool useSSL
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string userSMTP
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string pwdSMTP
        {
            get;
            set;
        }

        /// <summary>
        /// indirizzo mail del mittente dell'alert
        /// </summary>
        public string mailFrom
        {
            get;
            set;
        }

        /// <summary>
        /// indirizzo mail del destinatario dell'alert
        /// </summary>
        public string mailTo
        {
            get;
            set;
        }

        #endregion
    }

    #endregion

    #region SAVE

    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class SaveGestioneAlertRequest : PguServiceRequest
    {
        /// <summary>
        /// 
        /// </summary>
        public string idAmm
        {
            get;
            set;
        }

        #region ALERT
        /// <summary>
        /// Attivazione alert esecuzione anticipata verifica leggiblità
        /// </summary>
        public bool abilitatoVerificaAnt
        {
            get;
            set;
        }

        /// <summary>
        /// Attivazione alert esecuzione verifica leggibilità su campione non consentito
        /// </summary>
        public bool abilitatoVerificaPerc
        {
            get;
            set;
        }

        /// <summary>
        /// Attivazione alert frequenza utilizzo verifica leggibilità singolo doc
        /// </summary>
        public bool abilitatoVerificaDoc
        {
            get;
            set;
        }

        /// <summary>
        /// Attivazione alert frequenza utilizzo download istanza
        /// </summary>
        public bool abilitatoDownload
        {
            get;
            set;
        }

        /// <summary>
        /// Attivazione alert frequenza utilizzo sfoglia istanza
        /// </summary>
        public bool abilitatoSfoglia
        {
            get;
            set;
        }

        /// <summary>
        /// Campione massimo doc verificabili in una verifica di leggibilità
        /// </summary>
        public string maxVerificaPerc
        {
            get;
            set;
        }

        /// <summary>
        /// Numero massimo operazioni verifica leggibilità singolo doc
        /// </summary>
        public string maxOperVerificaDoc
        {
            get;
            set;
        }

        /// <summary>
        /// Periodo monitoraggio verifica leggibilità singolo doc
        /// </summary>
        public string periodoMonVerificaDoc
        {
            get;
            set;
        }

        /// <summary>
        /// Numero massimo operazioni download istanza
        /// </summary>
        public string maxOperDownload
        {
            get;
            set;
        }

        /// <summary>
        /// Periodo monitoraggio download istanza
        /// </summary>
        public string periodoMonDownload
        {
            get;
            set;
        }

        /// <summary>
        /// Numero massimo operazioni sfoglia istanza
        /// </summary>
        public string maxOperSfoglia
        {
            get;
            set;
        }

        /// <summary>
        /// Periodo monitoraggio sfoglia istanza
        /// </summary>
        public string periodoMonSfoglia
        {
            get;
            set;
        }
        #endregion

        #region configurazione mail

        /// <summary>
        /// Indirizzo server SMTP
        /// </summary>
        public string serverSMTP
        {
            get;
            set;
        }

        /// <summary>
        /// Porta server SMTP
        /// </summary>
        public string portaSMTP
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool useSSL
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string userSMTP
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string pwdSMTP
        {
            get;
            set;
        }

        /// <summary>
        /// indirizzo mail del mittente dell'alert
        /// </summary>
        public string mailFrom
        {
            get;
            set;
        }

        /// <summary>
        /// indirizzo mail del destinatario dell'alert
        /// </summary>
        public string mailTo
        {
            get;
            set;
        }

        #endregion

    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class SaveGestioneAlertResponse : PguServiceResponse
    {

    }

    #endregion

    #region UPDATE

    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class UpdateGestioneAlertRequest : PguServiceRequest
    {

        /// <summary>
        /// 
        /// </summary>
        public string idAmm
        {
            get;
            set;
        }

        #region ALERT
        /// <summary>
        /// Attivazione alert esecuzione anticipata verifica leggiblità
        /// </summary>
        public bool abilitatoVerificaAnt
        {
            get;
            set;
        }

        /// <summary>
        /// Attivazione alert esecuzione verifica leggibilità su campione non consentito
        /// </summary>
        public bool abilitatoVerificaPerc
        {
            get;
            set;
        }

        /// <summary>
        /// Attivazione alert frequenza utilizzo verifica leggibilità singolo doc
        /// </summary>
        public bool abilitatoVerificaDoc
        {
            get;
            set;
        }

        /// <summary>
        /// Attivazione alert frequenza utilizzo download istanza
        /// </summary>
        public bool abilitatoDownload
        {
            get;
            set;
        }

        /// <summary>
        /// Attivazione alert frequenza utilizzo sfoglia istanza
        /// </summary>
        public bool abilitatoSfoglia
        {
            get;
            set;
        }

        /// <summary>
        /// Campione massimo doc verificabili in una verifica di leggibilità
        /// </summary>
        public string maxVerificaPerc
        {
            get;
            set;
        }

        /// <summary>
        /// Numero massimo operazioni verifica leggibilità singolo doc
        /// </summary>
        public string maxOperVerificaDoc
        {
            get;
            set;
        }

        /// <summary>
        /// Periodo monitoraggio verifica leggibilità singolo doc
        /// </summary>
        public string periodoMonVerificaDoc
        {
            get;
            set;
        }

        /// <summary>
        /// Numero massimo operazioni download istanza
        /// </summary>
        public string maxOperDownload
        {
            get;
            set;
        }

        /// <summary>
        /// Periodo monitoraggio download istanza
        /// </summary>
        public string periodoMonDownload
        {
            get;
            set;
        }

        /// <summary>
        /// Numero massimo operazioni sfoglia istanza
        /// </summary>
        public string maxOperSfoglia
        {
            get;
            set;
        }

        /// <summary>
        /// Periodo monitoraggio sfoglia istanza
        /// </summary>
        public string periodoMonSfoglia
        {
            get;
            set;
        }
        #endregion

        #region configurazione mail

        /// <summary>
        /// Indirizzo server SMTP
        /// </summary>
        public string serverSMTP
        {
            get;
            set;
        }

        /// <summary>
        /// Porta server SMTP
        /// </summary>
        public string portaSMTP
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool useSSL
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string userSMTP
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string pwdSMTP
        {
            get;
            set;
        }

        /// <summary>
        /// indirizzo mail del mittente dell'alert
        /// </summary>
        public string mailFrom
        {
            get;
            set;
        }

        /// <summary>
        /// indirizzo mail del destinatario dell'alert
        /// </summary>
        public string mailTo
        {
            get;
            set;
        }

        #endregion

    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class UpdateGestioneAlertResponse : PguServiceResponse
    {

    }

    #endregion

    #endregion

    #region SaveFormati
    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class SaveFormatiRequest : PguServiceRequest
    {
        /// <summary>
        /// check ammesso per la conservazione
        /// </summary>
        public bool FileTypePreservation
        {
            get;
            set;
        }

        /// <summary>
        /// check validazione
        /// </summary>
        public bool FileTypeValidation
        {
            get;
            set;
        }

        /// <summary>
        /// system id del formato file
        /// </summary>
        public string SystemId
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class SaveFormatiResponse : PguServiceResponse
    {
    }
    #endregion
}