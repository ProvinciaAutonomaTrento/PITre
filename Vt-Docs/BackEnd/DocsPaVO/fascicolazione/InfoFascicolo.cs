using System;
using System.Collections;
using System.Xml.Serialization;

namespace DocsPaVO.fascicolazione
{
    [Serializable()]
	public class InfoFascicolo 
	{
        /// <summary>
        /// id univoco nodo ci classificazione cui appertiene il fascicolo
        /// </summary>
		public string idClassificazione;
        /// <summary>
        /// id univoco del fascicolo
        /// </summary>
		public string idFascicolo;
        /// <summary>
        /// codice registro associato al fascicolo
        /// </summary>
		public string codRegistro;
        /// <summary>
        /// id univoco del registro associato al fascicolo
        /// </summary>
        /// <remarks>può essere null</remarks>
		public string idRegistro;
        /// <summary>
        /// codice del fascicolo
        /// </summary>
		public string codice;
        /// <summary>
        /// descrizione del fascicolo
        /// </summary>
		public string descrizione;
        /// <summary>
        /// descrizione nodo classificazione
        /// </summary>
		public string descClassificazione;
        /// <summary>
        /// data di apertura
        /// </summary>
		public string apertura;

        /// <summary>
        /// Codice univoco dell'applicazione di appartenenza
        /// </summary>
        public string codiceApplicazione;

        //modifica
        public string contatore;
        //fine modifica

        public string privato = "0";
		/// <summary>
		/// aggiunta definizione costruttore per 
		/// anomalia 137 -- visualizzazione fascicoli
		/// trasmessi da link . PA 25102004
		/// </summary>
		/// <param name="objFascicolo"></param>
		public InfoFascicolo(Fascicolo objFascicolo) 
		{
			this.idClassificazione = objFascicolo.idClassificazione;
			this.idFascicolo = objFascicolo.systemID;
			this.codice = objFascicolo.codice;
			this.descrizione = objFascicolo.descrizione;
			this.apertura = objFascicolo.apertura;
            this.contatore = objFascicolo.contatore;
            this.codiceApplicazione = objFascicolo.codiceApplicazione;
		}
		/// <summary>
		/// aggiunto costruttore pre compatibilità codice. PA 25102204
		/// </summary>
		public InfoFascicolo()
		{}

	}


}
