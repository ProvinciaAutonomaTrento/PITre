// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DocsPaVO.utente 
{
	/// <summary>
	/// </summary>
    [Serializable()]
    [DataContract]
    public class Corrispondente
	{
        /// <summary>
        /// id univico del corrispondente
        /// </summary>
        [DataMember]
        public string systemId { get; set; }
        /// <summary>
        /// descrizione
        /// </summary>
        [DataMember]
        public string descrizione { get; set; }
        /// <summary>
        /// codice corrispondente
        /// </summary>
        /// <remarks>coincide con Codice Rubrica</remarks>
        [DataMember]
        public string codiceCorrispondente { get; set; }
        /// <summary>
        /// codice rubrica
        /// </summary>
        /// <remarks>utilizzabile per le ricerche</remarks>
        [DataMember]
        public string codiceRubrica { get; set; }
        /// <summary>
        /// id univico dell'amministrazione
        /// </summary>
        [DataMember]
        public string idAmministrazione { get; set; }
        /// <summary>
        /// indica il tipo del corrispondente
        /// </summary>
        /// <remarks>[S=singolo;O=occasionale;null]</remarks>
        [DataMember]
        public string tipoCorrispondente { get; set; }
        /// <summary>
        /// indica se il corr. � E=esterno all'amministrazione, o I=interno
        /// </summary>
        /// <remarks>se tipoCorrispondente=O=occasianale, allora tipoIE=null</remarks>
        [DataMember]
        public string tipoIE { get; set; }
        /// <summary>
        /// id univico del registro AOO cui appartiene il corrispondente
        /// </summary>
        [DataMember]
        public string idRegistro { get; set; }
        /// <summary>
        /// canele preferenziale di comunicazione
        /// </summary>
        [DataMember]
        public Canale canalePref { get; set; }
        /// <summary>
        /// true= l'oggetto dettagli contiene dei dettagli aggiuntivi
        /// </summary>
        [DataMember]
        public bool dettagli { get; set; }
        /// <summary>
        /// oggetto che descrive i dettagli aggiuntivi del corrispondente
        /// </summary>
        [DataMember]
        public System.Data.DataSet info { get; set; }
        /// <summary>
        /// deprecato
        /// </summary>
        [DataMember]
        public ServerPosta serverPosta { get; set; }
        /// <summary>
        /// id univico della precedente versione storicizzata del corrispondente
        /// </summary>
        [DataMember]
        public string idOld { get; set; }
        /// <summary>
        /// indirizzo mail
        /// </summary>
        [DataMember]
        public string email { get; set; }
        /// <summary>
        /// indirizzo mail alternativo
        /// </summary>
        /// <remarks>se !=null viene utilizzato nel campo From delle mail al posto dell'indirizzo nel campo email.</remarks>
        [DataMember]
        public string fromEmail { get; set; }
        /// <summary>
        /// indica se nella notifica via mail sono presenti anche gli allegati.
        /// </summary>
        [DataMember]
        public bool notificaConAllegato { get; set; }
        [DataMember]
        public string errore { get; set; }
        /// <summary>
        /// codice AOO
        /// </summary>
        /// <remarks>utilizzato per l'Interoperabilita</remarks>
        [DataMember]
        public string codiceAOO { get; set; }
        /// <summary>
        /// codice amministrazione
        /// </summary>
        /// <remarks>utilizzato per l'Interoperabilita</remarks>
        [DataMember]
        public string codiceAmm { get; set; }
        /// <summary>
        /// data di fine validit�
        /// </summary>
        /// <remarks>se !=null, allora il corrispondente non � storicizzato e non � pi� ricercabile nella rubrica</remarks>
        [DataMember]
        public string dta_fine { get; set; }

        /// <summary>
        /// nome
        /// </summary>
        /// <remarks>valido solo se utente</remarks>
        [DataMember]
        public string nome { get; set; } = string.Empty;
        /// <summary>
        /// cognome
        /// </summary>
        /// <remarks>valido solo se utente</remarks>
        [DataMember]
        public string cognome { get; set; } = string.Empty;
        [DataMember]
        public string indirizzo { get; set; } = string.Empty;
        /// <summary>
        /// citt�
        /// </summary>
        [DataMember]
        public string citta { get; set; } = string.Empty;
        /// <summary>
        /// cap
        /// </summary>
        [DataMember]
        public string cap { get; set; } = string.Empty;
        [DataMember]
        public string prov { get; set; } = string.Empty;
        [DataMember]
        public string nazionalita { get; set; } = string.Empty;
        [DataMember]
        public string telefono1 { get; set; } = string.Empty;
        [DataMember]
        public string telefono2 { get; set; } = string.Empty;
        [DataMember]
        public string fax { get; set; } = string.Empty;
        /// <summary>
        /// codice fiscale
        /// </summary>
        [DataMember]
        public string codfisc { get; set; } = string.Empty;
        /// <summary>
        /// partita iva
        /// </summary>
        [DataMember]
        public string partitaiva { get; set; } = string.Empty;
        [DataMember]
        public string note { get; set; } = string.Empty;

        /// <summary>
        /// Se true, indica che il corrispondente proviene da rubrica comune
        /// </summary>
        [DataMember]
        public bool inRubricaComune { get; set; }

        [DataMember]
        public documento.ProtocolloDestinatario protocolloDestinatario { get; set; }

        public override string ToString()
        {
            if (this.descrizione != null && this.codiceRubrica != null)
                return string.Format("{0} ({1})", this.descrizione, this.codiceRubrica);
            else
                return base.ToString();
        }
        [DataMember]
        public string codDescAmministrizazione { get; set; } = string.Empty;

        [DataMember]
        public string localita { get; set; } = string.Empty;

        [DataMember]
        public string luogoDINascita { get; set; } = string.Empty;

        [DataMember]
        public string dataNascita { get; set; } = string.Empty;

        [DataMember]
        public string titolo { get; set; } = string.Empty;

        [DataMember]
        public string oldDescrizione { get; set; } = string.Empty;
        /// <summary>
        /// Corrispondente disabilitato alla ricezione delle trasmissioni 
        /// </summary>
        [DataMember]
        public bool disabledTrasm { get; set; } = false;

        [DataMember]
        public bool interoperanteRGS { get; set; } = false;

        [DataMember]
        public string rubricaEsterna { get; set; } = string.Empty;

        private List<UrlInfo> _urls = new List<UrlInfo>();
        /// <summary>
        /// Url dell'istanza in cui � registrato il corrispondente. Viene utilizzato 
        /// per attivare l'Interoperabilita semplificata. Attualmente l'indirizzo pu�
        /// essere uno solo ma il sistema � predisposto ad accettarne pi� di uno. 
        /// Per rendere attiva la gestione multi url, modificare la Rubrica Comune ed
        /// il connettore con DB DocsPa
        /// </summary>
        [DataMember]
        public List<UrlInfo> Url 
        {
            get { return this._urls; }
            set
            {
                if (value == null)
                    value = new List<UrlInfo>();

                // Viene salvato solo se il corrispondente � una UO o un RF
                if (!(this is Utente))
                    this._urls = value;
            }
        }

        /// <summary>
        /// Informazioni sull'url associato ad un corrispondente
        /// </summary>
        [Serializable]
        [DataContract]
        public class UrlInfo
        {
            // Indirizzo url associato al corrispondente
            [DataMember]
            public String Url { get; set; }
            
        }

        private List<MailCorrispondente> _mailsCorr = new List<MailCorrispondente>();
        [DataMember]
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
