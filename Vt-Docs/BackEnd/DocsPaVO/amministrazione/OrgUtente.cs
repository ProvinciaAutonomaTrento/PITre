using System;
using System.Collections.Generic;

namespace DocsPaVO.amministrazione
{
	/// <summary>
	/// Definizione oggetto Utente 
	/// relativo alla funzionalità Utenti/Organigramma in Amministrazione.
	/// </summary>
	public class OrgUtente
	{		
		public string IDCorrGlobale = string.Empty;

		public string IDPeople = string.Empty;

		public string UserId = string.Empty;

		public string Codice = string.Empty;

		public string CodiceRubrica = string.Empty;

		public string Nome = string.Empty;

		public string Cognome = string.Empty;		

		public string Email = string.Empty;

        public string FromEmail = string.Empty;

		public string Sede = string.Empty;

		public string Password = string.Empty;

        public bool NessunaScadenzaPassword = false;

		public string Abilitato = string.Empty;		

		public string Dominio = string.Empty;

		public string Amministratore = string.Empty;

		public string NotificaTrasm = string.Empty;

		public string IDAmministrazione = string.Empty;

		public bool Automatico = false;

        /// <summary>
        ///  Indica, se true, che l'utente è soggetto alla sincronizzazione da LDAP.
        ///  L'opzione è valida solo se:
        ///  - la sincronizzazione ldap è abilitata per l'amministrazione
        ///  - l'utente non è un amministratore. Infatti, tramite quest'opzione, 
        ///  è possibile fare in modo  che un utente normale non sia soggetto alla sincronizzazione.
        /// </summary>
        public bool SincronizzaLdap = true;

        /// <summary>
        /// Id utilizzato per determinare l'univocità dell'utente nell'ambito della sincronizzazione LDAP.
        /// <remarks>
        /// Alcune amministrazioni potrebbero utilizzare, in LDAP, un nome utente differente dalla corrispondente 
        /// UserId in docspa (es, la matricola). L'attributo consente pertanto di gestire correttamente la duplicazione.
        /// Se non specificato, deve corrispondere alla UserId.
        /// </remarks>
        /// </summary>
        public string IdSincronizzazioneLdap = string.Empty;

        /// <summary>
        /// Indica, se true, che l'utente effettua l'autenticazione in LDAP.
        /// </summary>
        public bool AutenticatoInLdap = false;

        /// <summary>
        /// Identificativo del motore di elaborazione client side per la generazione dei modelli 
        /// </summary>
        public int IdClientSideModelProcessor = 0;

        /// <summary>
        /// Informazioni di profilo utente sull'utilizzo dei componenti SmartClient 
        /// </summary>
        public DocsPaVO.SmartClient.SmartClientConfigurations SmartClientConfigurations = new SmartClient.SmartClientConfigurations();
        /// <summary>
        /// Informazioni sul dispositivo stampa etichetta utilizzato correntemente dall'utente
        /// </summary>
        //public DocsPaVO.amministrazione.DispositivoStampaEtichetta DispositivoStampa = new DispositivoStampaEtichetta();
        public int? DispositivoStampa = null;

        /// <summary>
        /// Indica, se true, che l'utente sarà abilitato al Centro Servizi della conservazione
        /// </summary>
        public bool AbilitatoCentroServizi = false;

        /// <summary>
        /// Indica, se true, che l'utente sarà abilitato al Centro Servizi della conservazione
        /// </summary>
        public bool AbilitatoChiaviConfigurazione = false;

        /// <summary>
        /// Indica la matricola associata all'utente
        /// </summary>
        public string Matricola;

        /// <summary>
        /// Lista delle qualifiche dell'utente nel ruolo
        /// </summary>
        public List<DocsPaVO.Qualifica.PeopleGroupsQualifiche> Qualifiche
        {
            get;
            set;
        }

        /// <summary>
        /// Campo per l'esibizione
        /// Indica, se true, che l'utente sarà abilitato al Centro Servizi della conservazione
        /// </summary>
        public bool AbilitatoEsibizione = false;
	}
}
