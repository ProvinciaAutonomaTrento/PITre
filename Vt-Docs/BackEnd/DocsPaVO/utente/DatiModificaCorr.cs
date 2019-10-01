using System;
using System.Collections;
using System.Xml.Serialization;
using System.Collections.Generic;


namespace DocsPaVO.utente 
{
	/// <summary>
	/// </summary>
    [Serializable()]
	public class DatiModificaCorr 
	{	
        public string idCorrGlobali;
		public string descCorr;
        public string codiceAoo;
		public string codiceAmm;
		public string email;
		public string nome;
		public string cognome;
		public string codRubrica;
		public string codice;
		public string indirizzo;
		public string cap;
		public string provincia;
		public string nazione;
		public string codFiscale;
        public string partitaIva;
		public string fax;
		public string telefono;
		public string telefono2;
		public string note;
		public string citta;
		public string idCanalePref;
        public string tipoCorrispondente;
        public bool inRubricaComune;
        public string localita;
        public string luogoNascita;
        public string dataNascita;
        public string titolo;
        public string descrizioneCanalePreferenziale;
        public string idRegistro;
        public List<Corrispondente.UrlInfo> Urls { get; set; }
        public bool interoperanteRGS;
	}
}