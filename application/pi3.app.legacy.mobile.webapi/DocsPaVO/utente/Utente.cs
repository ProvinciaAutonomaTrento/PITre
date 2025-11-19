// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace DocsPaVO.utente 
{
	/// <summary>
	/// 
	/// </summary>
    [Serializable()]
    [DataContract]
	public class Utente : Corrispondente 
	{
        /// <summary>
        /// id univoco dell'oggetto utente
        /// </summary>
        /// <remarks>PEOPLE.SYSTEM_ID</remarks>
        [DataMember]
        public string idPeople { get; set; }
        /// <summary>
        /// token di autenticazione
        /// </summary>
        [DataMember]
        public string dst { get; set; }
        /// <summary>
        /// userID dell'utente
        /// </summary>
        [DataMember]
        public string userId { get; set; }
        /// <summary>
        /// indica se � abilitata la notitfica mail per l'utente
        /// </summary>
        /// <remarks>['E' =notifica con solo Link nella mail;null=nessuna notifica;'1'=notifica via mail con link e allegati</remarks>
        [DataMember]
        public string notifica { get; set; }
        [DataMember]
        public string telefono { get; set; }
        /// <summary>
        /// nome dell'utente
        /// </summary>
        [DataMember]
        public string nome { get; set; }
        /// <summary>
        /// cognome dell'utente
        /// </summary>
        [DataMember]
        public string cognome { get; set; }
        /// <summary>
        /// dominio dell'utente
        /// </summary>
        [DataMember]
        public string dominio { get; set; }
        /// <summary>
        /// indica se un utente � amministratore
        /// </summary>
        /// <remarks>True= amministrtore</remarks>
        [DataMember]
        public bool amministratore { get; set; }
        /// <summary>
        /// deprecato
        /// </summary>
        [DataMember]
        public bool assegnante { get; set; }
        /// <summary>
        /// deprecato
        /// </summary>
        [DataMember]
        public bool assegnatario { get; set; }
        /// <summary>
        /// sede dell'utente
        /// </summary>
        [DataMember]
        public string sede { get; set; }
        /// <summary>
        /// url dell'applicazione
        /// </summary>
        [DataMember]
        public string urlWA { get; set; } //url della wa dove � loggato l'utente serve per il link nelle mail delle trasmissioni

        /// <summary>
        /// arraylist oggetti Ruolo che rappresenta i ruoli dell'utente
        /// </summary>
		[XmlArray()]
		[XmlArrayItem(typeof(DocsPaVO.utente.Ruolo))]
        [DataMember]
        public System.Collections.ArrayList ruoli { get; set; }

        /// <summary>
        /// sessionId=dst
        /// </summary>
        [DataMember]
        public string sessionID { get; set; }

        /// <summary>
        /// arraylist oggetti PeopleGroupsQualifiche.cs che rappresenta le qualifiche dell'utente
        /// </summary>
        [XmlArray()]
        [XmlArrayItem(typeof(DocsPaVO.Qualifica.PeopleGroupsQualifiche))]
        [DataMember]
        public System.Collections.ArrayList qualifiche { get; set; }

        /// <summary>
        /// Indica la matricola associata all'utente
        /// </summary>
        [DataMember]
        public string matricola { get; set; }

        /// <summary>
        /// arraylist oggetti extApplication che rappresenta le applicazioni per l'utente
        /// </summary>
        [XmlArray()]
        [XmlArrayItem(typeof(DocsPaVO.utente.ExtApplication))]
        [DataMember]
        public System.Collections.ArrayList extApplications { get; set; }

        /// <summary>
        /// Indica l'appicazione su cui sta lavorando
        /// </summary>
        [DataMember]
        public string codWorkingApplication { get; set; }

        /// <summary>
        /// Indica se l'utente � disabilitato
        /// </summary>
        [DataMember]
        public string disabilitato { get; set; }
    }
}