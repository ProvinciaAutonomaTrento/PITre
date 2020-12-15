using System;
using System.Xml.Serialization;
using System.Collections;
using DocsPaVO.utente;
using DocsPaVO.documento;

//Andrea
using System.Collections.Generic;
//End Andrea

namespace DocsPaVO.trasmissione
{
	/// <summary>
	/// </summary>
	public class Trasmissione : OggettoTrasm 
	{
		[XmlArray()]
		[XmlArrayItem(typeof(DocsPaVO.trasmissione.TrasmissioneSingola))]
		public ArrayList trasmissioniSingole;

		public Utente utente;		
		public Ruolo ruolo;
		public static Hashtable oggettoStringa;
		public TipoOggetto tipoOggetto;
		public string noteGenerali;
		public string dataInvio;
		public string systemId;
        public CessioneDocumento cessione;
		public bool daAggiornare = false;
		public static ArrayList al;
        public bool salvataConCessione = false;     // "true" quando si salva una trasmissione con cessione dei diritti
        public bool mantieniLettura = false;     //se a true mantiene i diritti di lettura
        //
        // Mev Cessione diritti, mantenemdo anche i diritti di scrittura
        public bool mantieniScrittura = false;
        // End Mev
        //
        public string delegato;
        public string NO_NOTIFY; //LA TRASMISSIONE non attiva le notifiche in tdl e  non invia le notifiche vi mail
        public bool dirittiCeduti = false;
        public string[] listaDocControllati;
        
        //Andrea Prova
        public List<string> listaDestinatariNonRaggiungibili;
        //End Andrea

		/// <summary>
		/// Flag per la notifica di eventuali errori nell'inoltro delle email
		/// </summary>
		private bool errorSendingEmails = false;

		public bool ErrorSendingEmails
		{
			get { return this.errorSendingEmails;  }
			set { this.errorSendingEmails = value; }
		}

		/// <summary>
		/// </summary>
		public Trasmissione()
		{
			trasmissioniSingole = new System.Collections.ArrayList();

            //Andrea
            listaDestinatariNonRaggiungibili = new List<string>();
            //End Andrea

			if(oggettoStringa==null)
			{
				oggettoStringa=new System.Collections.Hashtable();
				oggettoStringa.Add(TipoOggetto.DOCUMENTO,"D");
				oggettoStringa.Add(TipoOggetto.FASCICOLO,"F");		   
			}
		}


        /// <summary>
        /// L'id della segnatura o il codice fascicolo
        /// Aggiunto per visualizzare il codice del fascicolo
        /// o la segnatura del documento anche nel caso in cui all'utente
        /// siano stati tolti i diritti di visiblità su questa trasmissione
        /// </summary>
        public string IdSegnaturaOCodFasc
        {
            get;
            set;
        }

        /// <summary>
        /// La data di creazione del documento o del fascicolo
        /// </summary>
        public string DataDocFasc
        {
            get;
            set;
        }
	}	
}