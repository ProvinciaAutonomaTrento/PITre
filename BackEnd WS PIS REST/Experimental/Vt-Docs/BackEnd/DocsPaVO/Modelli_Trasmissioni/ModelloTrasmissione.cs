using System;
using System.Xml.Serialization;

namespace DocsPaVO.Modelli_Trasmissioni
{
	public class ModelloTrasmissione
	{
		
		public int SYSTEM_ID;
		public string ID_AMM;
		public string NOME;
		public string CHA_TIPO_OGGETTO;
		public string ID_REGISTRO;
		public string VAR_NOTE_GENERALI;
		public string SINGLE;
		public string ID_PEOPLE;
        public string CEDE_DIRITTI;
        public string ID_PEOPLE_NEW_OWNER;
        public string ID_GROUP_NEW_OWNER;
        public string CODICE;
        public string NO_NOTIFY; //il modello non attiva le notifiche in tdl e  non invia le notifiche vi mail
        public string MANTIENI_LETTURA;
        public string MANTIENI_SCRITTURA; // Aggiunto Per MEV Cessione Diritti - Mantieni Scrittura
        [XmlArray()]
        [XmlArrayItem(typeof(DocsPaVO.Modelli_Trasmissioni.MittDest))]
        public System.Collections.ArrayList MITTENTE = new System.Collections.ArrayList();
		//public DocsPaVO.Modelli_Trasmissioni.MittDest MITTENTE;

		[XmlArray()]
		[XmlArrayItem(typeof(DocsPaVO.Modelli_Trasmissioni.RagioneDest))]
		public System.Collections.ArrayList RAGIONI_DESTINATARI = new System.Collections.ArrayList();

        /// <summary>
        /// Se true, il modello trasmissione prevede di nascondere le versioni precedenti
        /// a quella corrente di un documento trasmesso
        /// </summary>
        /// <remarks>
        /// Applicabile solo ai modelli trasmissione per i documenti
        /// </remarks>
        //public bool NASCONDI_VERSIONI_PRECEDENTI;

        /// <summary>
        /// True se il modello è valido
        /// </summary>
        public bool Valid { get; set; }

        /// <summary>
        /// Numero dei mittenti del modello. Attualmente viene utilizzata durante il caricamento
        /// delle informazioni sui modelli di trasmissione utente
        /// </summary>
        public int NumMittenti { get; set; }
	
    }
}