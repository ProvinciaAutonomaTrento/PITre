using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using DocsPaVO.FormatiDocumento;
using DocsPaVO.ProfilazioneDinamica;

namespace DocsPaVO.Conservazione
{
    [Serializable()]
    public class Policy
    {
        public string system_id = string.Empty;
        public string tipo = string.Empty;
        public string nome = string.Empty;
        public string idTemplate = string.Empty;
        public string idStatoDiagramma = string.Empty;
        public string idRf = string.Empty;
        public string classificazione = string.Empty;
        public string idAmministrazione = string.Empty;
        public bool arrivo = false;
        public bool partenza = false;
        public bool interno = false;
        public bool grigio = false;
        public bool automatico = false;
        public bool consolidazione = false;
        public string idAOO = string.Empty;
        public string dataCreazioneDa = string.Empty;
        public string dataCreazioneA = string.Empty;
        public string dataProtocollazioneDa = string.Empty;
        public string dataProtocollazioneA = string.Empty;
        public string tipoPeriodo = string.Empty;
        public string periodoGiornalieroNGiorni = string.Empty;
        public string periodoGiornalieroOre = string.Empty;
        public string periodoGiornalieroMinuti = string.Empty;
        public bool periodoSettimanaleLunedi = false;
        public bool periodoSettimanaleMartedi = false;
        public bool periodoSettimanaleMercoledi = false;
        public bool periodoSettimanaleGiovedi = false;
        public bool periodoSettimanaleVenerdi = false;
        public bool periodoSettimanaleSabato = false;
        public bool periodoSettimanaleDomenica = false;
        public string periodoSettimanaleOre = string.Empty;
        public string periodoSettimanaleMinuti = string.Empty;
        public string periodoMensileGiorni = string.Empty;
        public string periodoMensileOre = string.Empty;
        public string periodoMensileMinuti = string.Empty;
        public string idRuolo = string.Empty;
        public bool periodoAttivo = false;
        public string avvisoMesi = string.Empty;
        // MEV CS 1.5
        // scadenza verifiche leggibilità supporti
        public string avvisoMesiLegg = string.Empty;
        // fine MEV CS 1.5
        public string idUtenteRuolo = string.Empty;
        public string idGruppo = string.Empty;
        public string codiceUtente = string.Empty;
        public string tipoClassificazione = string.Empty;
        public string idUoCreatore = string.Empty;
        public string tipoDataCreazione = string.Empty;
        public string tipoDataProtocollazione = string.Empty;
        public bool uoSottoposte = false;
        public bool statoInviato = false;
        // MEV CS 1.5 F02_01
        // scadenza verifiche leggibilità supporti
        public bool statoConversione = false;
        // fine MEV CS 1.5 F02_01
        public bool includiSottoNodi = false;
        public bool soloDigitali = false;
        public bool soloFirmati = false;
        public string periodoAnnualeGiorno = string.Empty;
        public string periodoAnnualeMese = string.Empty;
        public string periodoAnnualeOre = string.Empty;
        public string periodoAnnualeMinuti = string.Empty;
        public string tipoConservazione = string.Empty;
        public Templates template = null;

        [XmlArray()]
        [XmlArrayItem(typeof(SupportedFileType))]
        public List<SupportedFileType> FormatiDocumento { get; set; }

        /// <summary>
        /// Funzione per l'aggiunta di un formato documento alla lista
        /// Il campo verrà aggiunto solo se non ne esiste già uno uguale
        /// </summary>
        /// <param name="fieldSettings"></param>
        public void AddSupportedFileType(SupportedFileType supportedFileType)
        {
            if (!this.FormatiDocumento.Contains(supportedFileType))
                this.FormatiDocumento.Add(supportedFileType);
        }

        /// <summary>
        /// Funzione per la rimozione di un formato documento dalla lista
        /// Il campo verrà eliminato solo se esiste nella lista
        /// </summary>
        /// <param name="fieldSettings"></param>
        public void DeleteSupportedFileType(SupportedFileType supportedFileType)
        {
            if (this.FormatiDocumento.Contains(supportedFileType))
                this.FormatiDocumento.Remove(supportedFileType);
        }

    }
}
