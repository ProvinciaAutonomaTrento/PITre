using System;
using System.Xml.Serialization;
using System.Collections;
using DocsPaVO.utente;

namespace DocsPaVO.trasmissione
{
	/// <summary>
	/// </summary>
	public class RagioneTrasmissione 
	{
		public string systemId;
		public string descrizione;
		public string tipo;
		public TipoDiritto tipoDiritti;
		public static Hashtable tipoDirittoStringa;
		public string risposta;
		public TipoGerarchia tipoDestinatario;
		public static Hashtable tipoGerarchiaStringa;
		public string note;
		public string eredita;
		public string tipoRisposta;
		public string notifica = string.Empty;
        public string mantieniLettura = string.Empty;

        //
        // MEV Cessione Diritti - mantieni Scrittura
        public string mantieniScrittura = string.Empty;
        // End MEV Cessione Diritti - mantieni Scrittura
        //
		
		public static string daVisualizzare="1";
		public static string daNonVisualizzare="0";

        public string testoMsgNotificaDoc = string.Empty;
       
        public string testoMsgNotificaFasc = string.Empty;

        public string prevedeCessione = string.Empty;   // possibili valori: "N", "W", "R"

        public bool cessioneImpostata = false; // usata quando la ragione prevede cessione ed ha il valore = "W": assume il valore "true" se l'utente ha impostato la cessione (spunta su trasm.ni) 

        public string azioneRichiesta = string.Empty;

        public bool fascicolazioneObbligatoria = false;

        public bool isTipoTask = false;
		/// <summary>
		/// </summary> 
		public RagioneTrasmissione()
		{
			if(tipoDirittoStringa==null)
			{
				tipoDirittoStringa=new Hashtable();
				tipoDirittoStringa.Add(TipoDiritto.READ,"R");
				tipoDirittoStringa.Add(TipoDiritto.WRITE,"W");
                tipoDirittoStringa.Add(TipoDiritto.NONE, "N");
			}

			if(tipoGerarchiaStringa==null)
			{
			    tipoGerarchiaStringa=new Hashtable();
				tipoGerarchiaStringa.Add(TipoGerarchia.INFERIORE,"I");
				tipoGerarchiaStringa.Add(TipoGerarchia.SUPERIORE,"S");
				tipoGerarchiaStringa.Add(TipoGerarchia.TUTTI,"T");
				tipoGerarchiaStringa.Add(TipoGerarchia.PARILIVELLO,"P");
			}
		}
	}
}