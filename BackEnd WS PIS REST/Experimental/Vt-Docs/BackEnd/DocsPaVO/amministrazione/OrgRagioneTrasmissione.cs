using System;

namespace DocsPaVO.amministrazione
{
	/// <summary>
	/// Rappresentazione di una ragione trasmissione
	/// </summary>
	public class OrgRagioneTrasmissione
	{
		public OrgRagioneTrasmissione()
		{
		}
		
		public string ID=string.Empty;

		public string IDAmministrazione=string.Empty;

		public string Codice=string.Empty;

		public string Descrizione=string.Empty;

		public TipiTrasmissioneEnum Tipo=TipiTrasmissioneEnum.SenzaWorkflow;

		public TipiNotificaTrasmissioneEnum TipoNotifica=TipiNotificaTrasmissioneEnum.Nessuna;

		public TipiDirittiTrasmissioneEnum TipoDiritto=TipiDirittiTrasmissioneEnum.LetturaScrittura;

		public TipiDestinatariTrasmissioneEnum TipoDestinatario=TipiDestinatariTrasmissioneEnum.Tutti;

		public bool Risposta=true;

		public bool Visibilita=false;

		public bool PrevedeRisposta=false;

		public bool Eredita=false;

        public bool MantieniLettura = false;

        public bool TipoTask = false;

        public string IdTipoAtto = string.Empty;

        public bool ContributoTaskObbligatorio = false;

        public bool ClassificazioneObbligatoria = false;

        //
        // Paramentro per la gestione dei diritti di scrittura
        public bool MantieniScrittura = false;
        // End
        //

        public string testoMsgNotificaDoc = string.Empty;

        public string testoMsgNotificaFasc = string.Empty;

        public CedeDiritiEnum PrevedeCessione = CedeDiritiEnum.No;

        public RagioneDiSistemaEnum DiSistema = RagioneDiSistemaEnum.No;

		/// <summary>
		/// Se true, la ragione di trasmissione è quella di default 
		/// in amministrazione per le trasmissoini automatiche ai destinatari
		/// </summary>
		public bool RagionePredefinitaDestinatari=false;

		/// <summary>
		/// Se true, la ragione di trasmissione è quella di default 
		/// in amministrazione per le trasmissoini automatiche ai destinatari in conoscenza
		/// </summary>
		public bool RagionePredefinitaDestinatariCC=false;

		public static string ParseTipoTrasmissione(TipiTrasmissioneEnum tipoTrasmissione)
		{
			string retValue="N";

			if (tipoTrasmissione==TipiTrasmissioneEnum.ConWorkflow)
				retValue="W";
			if (tipoTrasmissione==TipiTrasmissioneEnum.ConInterop)
				retValue="I";
            if (tipoTrasmissione == TipiTrasmissioneEnum.ConInteroperabilitaSemplificata)
                retValue = "S";

			return retValue;
		}

		public static TipiTrasmissioneEnum ParseTipoTrasmissione(string tipoTrasmissione)
		{
			TipiTrasmissioneEnum retValue=TipiTrasmissioneEnum.SenzaWorkflow;

			if (tipoTrasmissione=="W")
				retValue=TipiTrasmissioneEnum.ConWorkflow;
			if (tipoTrasmissione=="I")
				retValue=TipiTrasmissioneEnum.ConInterop;
            if (tipoTrasmissione == "S")
                retValue = TipiTrasmissioneEnum.ConInteroperabilitaSemplificata;

			return retValue;
		}

		public static string ParseTipoDiritto(TipiDirittiTrasmissioneEnum tipoDiritto)
		{
			string retValue="W";
			if (tipoDiritto==TipiDirittiTrasmissioneEnum.Lettura)
				retValue="R";
            if (tipoDiritto == TipiDirittiTrasmissioneEnum.Nessuno)
                return "N";
			return retValue;
		}

		public static TipiDirittiTrasmissioneEnum ParseTipoDiritto(string tipoDiritto)
		{
			TipiDirittiTrasmissioneEnum retValue=TipiDirittiTrasmissioneEnum.LetturaScrittura;
			if (tipoDiritto=="R")
				retValue=TipiDirittiTrasmissioneEnum.Lettura;
            if (tipoDiritto == "N")
                retValue = TipiDirittiTrasmissioneEnum.Nessuno;
			return retValue;
		}

		public static string ParseTipoDestinatario(TipiDestinatariTrasmissioneEnum tipoDestintatario)
		{
			string retValue="T";

			if (tipoDestintatario==TipiDestinatariTrasmissioneEnum.SoloSuperiori)
				retValue="S";
			else if (tipoDestintatario==TipiDestinatariTrasmissioneEnum.SoloSottposti)
				retValue="I";
			else if (tipoDestintatario==TipiDestinatariTrasmissioneEnum.Parilivello)
				retValue="P";
	
			return retValue;
		}

		public static TipiDestinatariTrasmissioneEnum ParseTipoDestinatario(string tipoDestintatario)
		{
			TipiDestinatariTrasmissioneEnum retValue=TipiDestinatariTrasmissioneEnum.Tutti;

			if (tipoDestintatario=="S")
				retValue=TipiDestinatariTrasmissioneEnum.SoloSuperiori;
			else if (tipoDestintatario=="I")
				retValue=TipiDestinatariTrasmissioneEnum.SoloSottposti;
			else if (tipoDestintatario=="P")
				retValue=TipiDestinatariTrasmissioneEnum.Parilivello;
	
			return retValue;
		}

		public static string ParseTipoNotifica(TipiNotificaTrasmissioneEnum tipoNotifica)
		{
			string retValue="Null";

            if (tipoNotifica == TipiNotificaTrasmissioneEnum.Mail)
                retValue = "E";
            else if (tipoNotifica == TipiNotificaTrasmissioneEnum.MailAllegati)
                retValue = "ED";
            else if (tipoNotifica == TipiNotificaTrasmissioneEnum.MailSoloAllegati)
                retValue = "EA";
            else if (tipoNotifica == TipiNotificaTrasmissioneEnum.NonNotificareMai)
                retValue = "NN";
			return retValue;
		}

		public static TipiNotificaTrasmissioneEnum ParseTipoNotifica(string tipoNotifica)
		{
			TipiNotificaTrasmissioneEnum retValue=TipiNotificaTrasmissioneEnum.Nessuna;

            if (tipoNotifica == "E")
                retValue = TipiNotificaTrasmissioneEnum.Mail;
            else if (tipoNotifica == "ED")
                retValue = TipiNotificaTrasmissioneEnum.MailAllegati;
            else if (tipoNotifica == "EA")
                retValue = TipiNotificaTrasmissioneEnum.MailSoloAllegati;
            else if (tipoNotifica == "NN")
                retValue = TipiNotificaTrasmissioneEnum.NonNotificareMai;
			return retValue;
		}

        public static CedeDiritiEnum ParseCedeDiritti(string prevedeCessione)
        {
            CedeDiritiEnum retValue = CedeDiritiEnum.No;    // no, mai.

            if (prevedeCessione == "W")
                retValue = CedeDiritiEnum.Si;               // sì: l'utente sceglie se attivare o meno la cessione dei diritti

            if (prevedeCessione == "R")
                retValue = CedeDiritiEnum.Sempre;           // sì sempre: opzione è sempre attiva, l'utente non ha scelta sull'opzione!

            return retValue;
        }

        public static string ParseCedeDiritti(CedeDiritiEnum enumCedeDiritti)
        {
            string retValue = "N";

            if (enumCedeDiritti == CedeDiritiEnum.Si)
                retValue = "W";

            if (enumCedeDiritti == CedeDiritiEnum.Sempre)
                retValue = "R";

            return retValue;
        }

        public static RagioneDiSistemaEnum ParseRagioneDiSistema(string ragioneDiSistema)
        {
            RagioneDiSistemaEnum retValue = RagioneDiSistemaEnum.No;

            if (ragioneDiSistema == "1")
                retValue = RagioneDiSistemaEnum.Si;

            return retValue;
        }

        public static string ParseRagioneDiSistema(RagioneDiSistemaEnum enumRagioneDiSistema)
        {
            string retValue = "0";

            if (enumRagioneDiSistema == RagioneDiSistemaEnum.Si)
                retValue = "1";

            return retValue;
        }

		/// <summary>
		/// Tipologie trasmissione
		/// </summary>
		public enum TipiTrasmissioneEnum
		{
			SenzaWorkflow,
			ConWorkflow,
			ConInterop,
            ConInteroperabilitaSemplificata
		}

		/// <summary>
		/// Tipologie di notifica trasmissione
		/// </summary>
		public enum TipiNotificaTrasmissioneEnum
		{
			Nessuna,
			Mail,
			MailAllegati,
            MailSoloAllegati,
            NonNotificareMai
		}

		/// <summary>
		/// Tipologie di diritti trasmissione
		/// </summary>
		public enum TipiDirittiTrasmissioneEnum
		{
			LetturaScrittura,
			Lettura,
            Cessione,
            Nessuno
		}

		/// <summary>
		/// Tipologie di destinatari trasmissione
		/// </summary>
		public enum TipiDestinatariTrasmissioneEnum
		{
			Tutti,
			SoloSuperiori,
			SoloSottposti,
			Parilivello
		}

        /// <summary>
        /// Tipologie di cessione diritti
        /// </summary>
        public enum CedeDiritiEnum
        {
            No,
            Si,
            Sempre
        }

        /// <summary>
        /// Tipologia di ragione di sistema
        /// </summary>
        public enum RagioneDiSistemaEnum
        {
            No,
            Si
        }
	}
}
