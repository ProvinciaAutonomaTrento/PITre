using System;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;

namespace DocsPaVO.Smistamento
{
	/// <summary>
	/// Definizione oggetto Ruolo 
	/// relativo alla funzionalità di smistamento documenti.
	/// </summary>
	public class RuoloSmistamento
	{
		public string ID=string.Empty;
		public string Codice=string.Empty;
		public string Descrizione=string.Empty;

		public bool RuoloRiferimento=false;

		[XmlArray()]
		[XmlArrayItem(typeof(DocsPaVO.Smistamento.UtenteSmistamento))]
		public ArrayList Utenti=new ArrayList();

		public bool FlagCompetenza=false;
		public bool FlagConoscenza=false;

        [XmlArray()]
        [XmlArrayItem(typeof(string))]
        public ArrayList Registri = new ArrayList();
        //public string NoteIndividuali = string.Empty;
        //public string DataScadenza = string.Empty;
        public DocsPaVO.Smistamento.datiAggiuntiviSmistamento datiAggiuntiviSmistamento= new datiAggiuntiviSmistamento();
        
        //indica se un Ruolo è superiore a quello che sta smistando il documento

        /// <summary>
        /// Indica il livello di gerarchia del ruolo rispetto al ruolo corrente dell'utente che sta smistando.
        /// La convenzione è:
        /// 0 --> Ruolo inferiore
        /// 1 --> Ruolo superiore
        /// 2 --> Pari livello
        /// </summary>
        /// <remarks>
        /// Il livello è importante per determinare, in sede di smistamento, se per il ruolo può essere
        /// trasmesso un documento in base alla ragione di trasmissione selezionata in amministrazione
        /// </remarks>
        public string Gerarchia = string.Empty;

        public string ragioneTrasmRapida = string.Empty;

        public string chaTipoTrasm = string.Empty;

        public bool disabledTrasm = false;
    }
}
