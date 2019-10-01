using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace DocsPaVO.OggettiLite
{
    
    [Serializable()]
	public class CorrispondenteLite
	{
        /// <summary>
        /// id univico del corrispondente
        /// </summary>
		public string systemId;
            
        /// <summary>
        /// codice rubrica
        /// </summary>
        /// <remarks>utilizzabile per le ricerche</remarks>
		public string codiceRubrica;
        /// <summary>
        /// descrizione
        /// </summary>
        public string descrizione;
        /// <summary>
        /// id univico dell'amministrazione
        /// </summary>
		public string idAmministrazione;
        /// <summary>
        /// indica il tipo del corrispondente
        /// </summary>
        /// <remarks>[U,P,R]</remarks>
		public string tipoCorrispondente;
       
        /// <summary>
        /// indirizzo
        /// </summary>
		public string indirizzo;
        /// <summary>
        /// cap
        /// </summary>
		public string cap;
        /// <summary>
        /// citta
        /// </summary>
		public string citta;
        /// <summary>
        /// provincia
        /// </summary>
		public string prov;
        /// <summary>
        /// nazione
        /// </summary>
		public string nazionalita;
        /// <summary>
        /// telefono
        /// </summary>
		public string telefono1;
        /// <summary>
        /// telefono2
        /// </summary>
		public string telefono2;
        /// <summary>
        /// fax
        /// </summary>
		public string fax;
        /// <summary>
        /// codiceFiscale
        /// </summary>
		public string codfisc;
       /// <summary>
        /// note
        /// </summary>
		public string note;
      /// <summary>
        /// localita
        /// </summary>
		public string localita;            
         /// <summary>
        /// indirizzo mail
        /// </summary>
		public string email;
         /// <summary>
        /// codice AOO
        /// </summary>
        /// <remarks>utilizzato per l'interoperabilità</remarks>
		public string codiceAOO;
        /// <summary>
        /// codice amministrazione
        /// </summary>
        /// <remarks>utilizzato per l'interoperabilità</remarks>
		public string codiceAmm;
        /// <summary>
        /// data di fine validità
        /// </summary>
        /// <remarks>se !=null, allora il corrispondente non è storicizzato e non è più ricercabile nella rubrica</remarks>
		public string dta_fine;      
       
         /// <summary>
        /// indica se il corr. è E=esterno all'amministrazione, o I=interno
        /// </summary>
        /// <remarks>se tipoCorrispondente=O=occasianale, allora tipoIE=null</remarks>
		public string tipoIE;
        /// <summary>
        /// id univico del registro AOO cui appartiene il corrispondente
        /// </summary>
		public string idRegistro;
    
    }
}


