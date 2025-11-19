// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DocsPaVO.amministrazione
{
    /// <summary>
    /// Definizione oggetto Utente 
    /// relativo alla funzionalit� Utenti/Organigramma in Amministrazione.
    /// </summary>
    [DataContract]
    public class OrgUtente
	{

        [DataMember]
        public string IDCorrGlobale { get; set; } = string.Empty;

        [DataMember]
        public string IDPeople { get; set; } = string.Empty;

        [DataMember]
        public string UserId { get; set; } = string.Empty;

        [DataMember]
        public string Codice { get; set; } = string.Empty;

        [DataMember]
        public string CodiceRubrica { get; set; } = string.Empty;

        [DataMember]
        public string Nome { get; set; } = string.Empty;

        [DataMember]
        public string Cognome { get; set; } = string.Empty;

        [DataMember]
        public string Email { get; set; } = string.Empty;

        [DataMember]
        public string FromEmail { get; set; } = string.Empty;

        [DataMember]
        public string Sede { get; set; } = string.Empty;

        [DataMember]
        public string Password { get; set; } = string.Empty;

        [DataMember]
        public bool NessunaScadenzaPassword { get; set; } = false;

        [DataMember]
        public string Abilitato { get; set; } = string.Empty;

        [DataMember]
        public string Dominio { get; set; } = string.Empty;

        [DataMember]
        public string Amministratore { get; set; } = string.Empty;

        [DataMember]
        public string NotificaTrasm { get; set; } = string.Empty;

        [DataMember]
        public string IDAmministrazione { get; set; } = string.Empty;

        [DataMember]
        public bool Automatico { get; set; } = false;

        /// <summary>
        ///  Indica, se true, che l'utente � soggetto alla sincronizzazione da LDAP.
        ///  L'opzione � valida solo se:
        ///  - la sincronizzazione ldap � abilitata per l'amministrazione
        ///  - l'utente non � un amministratore. Infatti, tramite quest'opzione, 
        ///  � possibile fare in modo  che un utente normale non sia soggetto alla sincronizzazione.
        /// </summary>
        [DataMember]
        public bool SincronizzaLdap { get; set; } = true;

        /// <summary>
        /// Id utilizzato per determinare l'univocit� dell'utente nell'ambito della sincronizzazione LDAP.
        /// <remarks>
        /// Alcune amministrazioni potrebbero utilizzare, in LDAP, un nome utente differente dalla corrispondente 
        /// UserId in docspa (es, la matricola). L'attributo consente pertanto di gestire correttamente la duplicazione.
        /// Se non specificato, deve corrispondere alla UserId.
        /// </remarks>
        /// </summary>
        [DataMember]
        public string IdSincronizzazioneLdap { get; set; } = string.Empty;

        /// <summary>
        /// Indica, se true, che l'utente effettua l'autenticazione in LDAP.
        /// </summary>
        [DataMember]
        public bool AutenticatoInLdap { get; set; } = false;

        /// <summary>
        /// Identificativo del motore di elaborazione client side per la generazione dei modelli 
        /// </summary>
        [DataMember]
        public int IdClientSideModelProcessor { get; set; } = 0;

        /// <summary>
        /// Informazioni di profilo utente sull'utilizzo dei componenti SmartClient 
        /// </summary>
        [DataMember]
        public DocsPaVO.SmartClient.SmartClientConfigurations SmartClientConfigurations { get; set; } = new SmartClient.SmartClientConfigurations();
        /// <summary>
        /// Informazioni sul dispositivo stampa etichetta utilizzato correntemente dall'utente
        /// </summary>
        //public DocsPaVO.amministrazione.DispositivoStampaEtichetta DispositivoStampa = new DispositivoStampaEtichetta();
        [DataMember]
        public int? DispositivoStampa { get; set; } = null;

        /// <summary>
        /// Indica, se true, che l'utente sar� abilitato al Centro Servizi della conservazione
        /// </summary>
        [DataMember]
        public bool AbilitatoCentroServizi { get; set; } = false;

        /// <summary>
        /// Indica, se true, che l'utente sar� abilitato al Centro Servizi della conservazione
        /// </summary>
        [DataMember]
        public bool AbilitatoChiaviConfigurazione { get; set; } = false;

        /// <summary>
        /// Indica la matricola associata all'utente
        /// </summary>
        [DataMember]
        public string Matricola { get; set; }

        /// <summary>
        /// Lista delle qualifiche dell'utente nel ruolo
        /// </summary>
        [DataMember]
        public List<DocsPaVO.Qualifica.PeopleGroupsQualifiche> Qualifiche
        {
            get;
            set;
        }

        /// <summary>
        /// Campo per l'esibizione
        /// Indica, se true, che l'utente sar� abilitato al Centro Servizi della conservazione
        /// </summary>
        [DataMember]
        public bool AbilitatoEsibizione { get; set; } = false;
	}
}
