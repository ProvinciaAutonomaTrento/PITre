using System;
using System.Xml.Serialization;
using System.Collections;

namespace DocsPaVO.rubrica
{
	/// <summary>
	/// La classe di base per la rubrica
	/// </summary>
	/// <remarks>
	/// Questa classe contiene unicamente i dati minimi necessari
	/// alla gestione delle ricerche e per la comunicazione
	/// backend / frontend
	/// </remarks>
	[Serializable()]
	[XmlType("ElementoRubrica")]
    [XmlInclude(typeof(RubricaComune.InfoElementoRubricaComune))]
	public class ElementoRubrica 
	{
        //systemId dell'elemento rubrica
        public string systemId;
        /// <summary>
		/// Il codice rubrica dell'elemento
		/// </summary>
		/// <remarks>
		/// Il codice rubrica dell'elemento
		/// </remarks>
		public string codice;

		/// <summary>
		/// La descrizione dell'elemento
		/// </summary>
		public string descrizione;

		/// <summary>
		/// Il tipo (Interno, Esterno, Interno/Esterno) dell'elemento.
		/// Può essere ripettivamente "I", "E" o "IE"
		/// </summary>
		public string tipo;

        /// <summary>
        /// Il canale preferenziale (Lettera, EMAIL, ecc...).
        /// </summary>
        public string canale;

		/// <summary>
		/// L'elemento è interno ? 1 | 0
		/// </summary>
		public bool interno;

		/// <summary>
		/// L'elemento ha figli ? 1 | 0
		/// </summary>
		public bool has_children;
		
		/// <summary>
		/// se true vuol dire che il corrispondente è selezionabile nel contensto corrente
		/// </summary>
		public bool isVisibile = true;


        public bool isRubricaComune;
        /// <summary>
        /// Mantiene le informazioni di rubrica comune qualora
        /// il corrispondente sia proveniente da rubrica comune
        /// </summary>
        public DocsPaVO.RubricaComune.InfoElementoRubricaComune rubricaComune;

        public string idRegistro;

        public string codiceRegistro;

        /// <summary>
        /// Classe per il confronto di due oggetti "ElementoRubrica" ai fini dell'ordinamento
        /// </summary>
        public class ElementoRubricaComparer : IComparer
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public int Compare(object x, object y)
            {
                // cha_tipo_urp desc, var_desc_corr";	

                ElementoRubrica elemX = (ElementoRubrica)x;
                ElementoRubrica elemY = (ElementoRubrica)y;

                return elemX.descrizione.ToUpper().Trim().CompareTo(elemY.descrizione.ToUpper().Trim());
            }
        }

        /// <summary>
        /// Corrispondente disabilitato alla ricezione delle trasmissioni 
        /// </summary>
        public bool disabledTrasm = false;

        /// <summary>
        /// Corrispondente disabilitato
        /// </summary>
        public bool disabled = false;

        /// <summary>
        /// Nome del corrispondente
        /// </summary>
        public string nome;

        /// <summary>
        /// Cognome del corrispondente
        /// </summary>
        public string cognome;

        /// <summary>
        /// Codice fiscale o p.iva
        /// </summary>
        public string cf_piva;

        /// <summary>
        /// Codice fiscale
        /// </summary>
        public string codiceFiscale;

        /// <summary>
        /// Partita iva
        /// </summary>
        public string partitaIva;





        /// <summary>
        /// Nel caso di utente id people
        /// </summary>
        public string idPeople;

        public string idPeopleLista;

        public string idGruppoLista;
	}
}
