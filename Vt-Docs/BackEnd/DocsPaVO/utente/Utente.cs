using System;
using System.Collections;
using System.Xml.Serialization;

namespace DocsPaVO.utente 
{
	/// <summary>
	/// 
	/// </summary>
    [Serializable()]
	public class Utente : Corrispondente 
	{
        /// <summary>
        /// id univoco dell'oggetto utente
        /// </summary>
        /// <remarks>PEOPLE.SYSTEM_ID</remarks>
		public string idPeople;
        /// <summary>
        /// token di autenticazione
        /// </summary>
		public string dst;
        /// <summary>
        /// userID dell'utente
        /// </summary>
		public string userId;
        /// <summary>
        /// indica se è abilitata la notitfica mail per l'utente
        /// </summary>
        /// <remarks>['E' =notifica con solo Link nella mail;null=nessuna notifica;'1'=notifica via mail con link e allegati</remarks>
		public string notifica;		
		public string telefono;
        /// <summary>
        /// nome dell'utente
        /// </summary>
		public string nome;
        /// <summary>
        /// cognome dell'utente
        /// </summary>
		public string cognome;
        /// <summary>
        /// dominio dell'utente
        /// </summary>
		public string dominio;
        /// <summary>
        /// indica se un utente è amministratore
        /// </summary>
        /// <remarks>True= amministrtore</remarks>
		public bool amministratore;
        /// <summary>
        /// deprecato
        /// </summary>
		public bool assegnante;
        /// <summary>
        /// deprecato
        /// </summary>
		public bool assegnatario;
        /// <summary>
        /// sede dell'utente
        /// </summary>
		public string sede;
        /// <summary>
        /// url dell'applicazione
        /// </summary>
		public string urlWA; //url della wa dove è loggato l'utente serve per il link nelle mail delle trasmissioni

        /// <summary>
        /// arraylist oggetti Ruolo che rappresenta i ruoli dell'utente
        /// </summary>
		[XmlArray()]
		[XmlArrayItem(typeof(DocsPaVO.utente.Ruolo))]
		public System.Collections.ArrayList ruoli;

        /// <summary>
        /// sessionId=dst
        /// </summary>
		public string sessionID;

        /// <summary>
        /// arraylist oggetti PeopleGroupsQualifiche.cs che rappresenta le qualifiche dell'utente
        /// </summary>
        [XmlArray()]
        [XmlArrayItem(typeof(DocsPaVO.Qualifica.PeopleGroupsQualifiche))]
        public System.Collections.ArrayList qualifiche;

        /// <summary>
        /// Indica la matricola associata all'utente
        /// </summary>
        public string matricola;

        /// <summary>
        /// arraylist oggetti extApplication che rappresenta le applicazioni per l'utente
        /// </summary>
        [XmlArray()]
        [XmlArrayItem(typeof(DocsPaVO.utente.ExtApplication))]
        public System.Collections.ArrayList extApplications;

        /// <summary>
        /// Indica l'appicazione su cui sta lavorando
        /// </summary>
        public string codWorkingApplication;

        /// <summary>
        /// Indica se l'utente è disabilitato
        /// </summary>
        public string disabilitato;
	}
}