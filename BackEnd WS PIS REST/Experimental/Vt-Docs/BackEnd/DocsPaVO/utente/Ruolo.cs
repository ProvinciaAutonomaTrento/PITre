using System;
using System.Collections;
using System.Xml.Serialization;

namespace DocsPaVO.utente 
{
	/// <summary>
	/// </summary>
    [Serializable()]
	public class Ruolo: Corrispondente 
	{
        /// <summary>
        /// livello del tipo Ruolo del Ruolo
        /// </summary>
		public string livello;
        /// <summary>
        /// codice rubrica
        /// </summary>
		public string codice;
        /// <summary>
        /// id univoco del Ruolo
        /// </summary>
        /// <remarks>GROUPS.SYSTEM_ID</remarks>
		public string idGruppo;
		public string codiceIstat;
        /// <summary>
        /// deprecato
        /// </summary>
		public bool selezionato = false;
        /// <summary>
        /// tipo Ruolo del ruolo
        /// </summary>
		public TipoRuolo tipoRuolo;
        /// <summary>
        /// uo Padre del ruolo
        /// </summary>
		public UnitaOrganizzativa uo;

        /// <summary>
        /// Booleano che indica se bisogna visualizzare il pulsante della storia del ruolo nella
        /// maschera di visibilità del documento.
        /// Il tipo String è stato utilizzato al posto di boolean per mantenere la compatibilità con
        /// chi utilizza gli smart services in quanto il tipo boolean produce un wsdl con minOccur 1
        /// </summary>
        public String ShowHistory { get; set; }

        /// <summary>
        /// arraylist delle funzioni abilitate
        /// </summary>
		[XmlArray()]
		[XmlArrayItem(typeof(DocsPaVO.utente.Funzione))]
		public System.Collections.ArrayList funzioni;

        /// <summary>
        /// arraylist dei registri associati al Ruolo
        /// </summary>
		[XmlArray()]
		[XmlArrayItem(typeof(DocsPaVO.utente.Registro))]
		public System.Collections.ArrayList registri;

        /// <summary>
        /// 
        /// </summary>
        public bool Responsabile = false;

        /// <summary>
        /// 
        /// </summary>
        public bool Segretario = false;

        /// <summary>
        /// Autenticazione Sistemi Esterni
        /// Se è un sistema esterno è 1
        /// </summary>
        public string RuoloDiSistema;

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