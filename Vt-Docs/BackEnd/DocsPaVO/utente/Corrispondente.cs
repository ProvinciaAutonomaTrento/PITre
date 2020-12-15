using System;
using System.Collections;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace DocsPaVO.utente 
{
	/// <summary>
	/// </summary>
    [Serializable()]
	public class Corrispondente
	{
        /// <summary>
        /// id univico del corrispondente
        /// </summary>
		public string systemId;
        /// <summary>
        /// descrizione
        /// </summary>
		public string descrizione;
        /// <summary>
        /// codice corrispondente
        /// </summary>
        /// <remarks>coincide con Codice Rubrica</remarks>
		public string codiceCorrispondente;
        /// <summary>
        /// codice rubrica
        /// </summary>
        /// <remarks>utilizzabile per le ricerche</remarks>
		public string codiceRubrica;
        /// <summary>
        /// id univico dell'amministrazione
        /// </summary>
		public string idAmministrazione;
        /// <summary>
        /// indica il tipo del corrispondente
        /// </summary>
        /// <remarks>[S=singolo;O=occasionale;null]</remarks>
		public string tipoCorrispondente;
        /// <summary>
        /// indica se il corr. è E=esterno all'amministrazione, o I=interno
        /// </summary>
        /// <remarks>se tipoCorrispondente=O=occasianale, allora tipoIE=null</remarks>
		public string tipoIE;
        /// <summary>
        /// id univico del registro AOO cui appartiene il corrispondente
        /// </summary>
		public string idRegistro;
        /// <summary>
        /// canele preferenziale di comunicazione
        /// </summary>
		public Canale canalePref;
        /// <summary>
        /// true= l'oggetto dettagli contiene dei dettagli aggiuntivi
        /// </summary>
		public bool   dettagli;
        /// <summary>
        /// oggetto che descrive i dettagli aggiuntivi del corrispondente
        /// </summary>
		public System.Data.DataSet info;
        /// <summary>
        /// deprecato
        /// </summary>
		public ServerPosta serverPosta;
        /// <summary>
        /// id univico della precedente versione storicizzata del corrispondente
        /// </summary>
		public string idOld;
        /// <summary>
        /// indirizzo mail
        /// </summary>
		public string email;
        /// <summary>
        /// indirizzo mail alternativo
        /// </summary>
        /// <remarks>se !=null viene utilizzato nel campo From delle mail al posto dell'indirizzo nel campo email.</remarks>
        public string fromEmail;
        /// <summary>
        /// indica se nella notifica via mail sono presenti anche gli allegati.
        /// </summary>
		public bool   notificaConAllegato;
		public string errore;
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
        /// nome
        /// </summary>
        /// <remarks>valido solo se utente</remarks>
        public string nome = string.Empty;
        /// <summary>
        /// cognome
        /// </summary>
        /// <remarks>valido solo se utente</remarks>
        public string cognome = string.Empty;
        public string indirizzo = string.Empty;
        /// <summary>
        /// città
        /// </summary>
        public string citta = string.Empty;
        /// <summary>
        /// cap
        /// </summary>
        public string cap = string.Empty;
        public string prov = string.Empty;
        public string nazionalita = string.Empty;
        public string telefono1 = string.Empty;
        public string telefono2 = string.Empty;
        public string fax = string.Empty;
        /// <summary>
        /// codice fiscale
        /// </summary>
        public string codfisc = string.Empty;
        /// <summary>
        /// partita iva
        /// </summary>
        public string partitaiva = string.Empty;
        public string note = string.Empty;

        /// <summary>
        /// Se true, indica che il corrispondente proviene da rubrica comune
        /// </summary>
        public bool inRubricaComune;

        public documento.ProtocolloDestinatario protocolloDestinatario; 

        public override string ToString()
        {
            if (this.descrizione != null && this.codiceRubrica != null)
                return string.Format("{0} ({1})", this.descrizione, this.codiceRubrica);
            else
                return base.ToString();
        }
        public string codDescAmministrizazione = string.Empty;

        public string localita = string.Empty;

        public string luogoDINascita = string.Empty;

        public string dataNascita = string.Empty;

        public string titolo = string.Empty;

        public string oldDescrizione = string.Empty;
        /// <summary>
        /// Corrispondente disabilitato alla ricezione delle trasmissioni 
        /// </summary>
        public bool disabledTrasm = false;

        public bool interoperanteRGS = false;

        private List<UrlInfo> _urls = new List<UrlInfo>();
        /// <summary>
        /// Url dell'istanza in cui è registrato il corrispondente. Viene utilizzato 
        /// per attivare l'interoperabilità semplificata. Attualmente l'indirizzo può
        /// essere uno solo ma il sistema è predisposto ad accettarne più di uno. 
        /// Per rendere attiva la gestione multi url, modificare la Rubrica Comune ed
        /// il connettore con DB DocsPa
        /// </summary>
        public List<UrlInfo> Url 
        {
            get { return this._urls; }
            set
            {
                if (value == null)
                    value = new List<UrlInfo>();

                // Viene salvato solo se il corrispondente è una UO o un RF
                if (!(this is Utente))
                    this._urls = value;
            }
        }

        /// <summary>
        /// Informazioni sull'url associato ad un corrispondente
        /// </summary>
        [Serializable]
        public class UrlInfo
        {
            // Indirizzo url associato al corrispondente
            public String Url { get; set; }
            
        }

        private List<MailCorrispondente> _mailsCorr = new List<MailCorrispondente>();
        public List<MailCorrispondente> Emails
        {
            get {
                return this._mailsCorr;
            }

            set {
                if (value == null)
                    this._mailsCorr = new List<MailCorrispondente>();
                this._mailsCorr = value;
            }
        }
	}
}
