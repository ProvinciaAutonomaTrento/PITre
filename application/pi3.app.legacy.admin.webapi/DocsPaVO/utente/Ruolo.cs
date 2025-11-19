// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace DocsPaVO.utente 
{
	/// <summary>
	/// </summary>
    [Serializable()]
	[DataContract]
	public class Ruolo: Corrispondente 
	{
		/// <summary>
		/// livello del tipo Ruolo del Ruolo
		/// </summary>
		[DataMember]
        public string livello { get; set; }
        /// <summary>
        /// codice rubrica
        /// </summary>
		[DataMember]
        public string codice { get; set; }
        /// <summary>
        /// id univoco del Ruolo
        /// </summary>
        /// <remarks>GROUPS.SYSTEM_ID</remarks>
        [DataMember]
        public string idGruppo { get; set; }
        [DataMember]
        public string codiceIstat { get; set; }
        /// <summary>
        /// deprecato
        /// </summary>
        [DataMember]
        public bool selezionato { get; set; } = false;
        /// <summary>
        /// tipo Ruolo del ruolo
        /// </summary>
        [DataMember]
        public TipoRuolo tipoRuolo { get; set; }
        /// <summary>
        /// uo Padre del ruolo
        /// </summary>
        [DataMember]
        public UnitaOrganizzativa uo { get; set; }

        /// <summary>
        /// Booleano che indica se bisogna visualizzare il pulsante della storia del ruolo nella
        /// maschera di visibilit� del documento.
        /// Il tipo String � stato utilizzato al posto di boolean per mantenere la compatibilit� con
        /// chi utilizza gli smart services in quanto il tipo boolean produce un wsdl con minOccur 1
        /// </summary>
		[DataMember]
        public String ShowHistory { get; set; }

        /// <summary>
        /// arraylist delle funzioni abilitate
        /// </summary>
		[DataMember]
        [XmlArray()]
		[XmlArrayItem(typeof(DocsPaVO.utente.Funzione))]
		public System.Collections.ArrayList funzioni { get; set; }

        /// <summary>
        /// arraylist dei registri associati al Ruolo
        /// </summary>
        [DataMember]
        [XmlArray()]
		[XmlArrayItem(typeof(DocsPaVO.utente.Registro))]
		public System.Collections.ArrayList registri { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public bool Responsabile { get; set; } = false;

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public bool Segretario { get; set; } = false;

        /// <summary>
        /// Autenticazione Sistemi Esterni
        /// Se � un sistema esterno � 1
        /// </summary>
        [DataMember]
        public string RuoloDiSistema { get; set; }

        /// <summary>
        /// </summary>
        public Ruolo()
		{
		}

		/// <summary>
		/// </summary>
		/// <param name="systemId"></param>
		/// <param name="descrizione"></param>
		/// <param name="codice"></param>
		/// <param name="livello"></param>
		/// <param name="idGruppo"></param>
		/// <param name="tipoRuolo"></param>
		/// <param name="funzioni"></param>
		public Ruolo(string systemId,
					 string descrizione,
					 string codice,
					 string livello,
					 string idGruppo,
					 TipoRuolo tipoRuolo,
					 System.Collections.ArrayList funzioni)
		{
			this.systemId=systemId;
			this.descrizione=descrizione;
			this.codice=codice;
			this.livello=livello;
			this.idGruppo=idGruppo;
			this.funzioni=funzioni;
			this.tipoRuolo=tipoRuolo;
			
		}
	}
}