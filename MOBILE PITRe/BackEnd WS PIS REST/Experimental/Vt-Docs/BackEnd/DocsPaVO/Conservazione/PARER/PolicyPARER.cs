using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.FormatiDocumento;
using DocsPaVO.ProfilazioneDinamica;
using System.Xml.Serialization;

namespace DocsPaVO.Conservazione.PARER
{
    [Serializable()]
    public class PolicyPARER
    {
        public string id;
        public string tipo;
        public string codice;
        public string descrizione;
        public string isAttiva;
        public string idGruppoRuoloResp;
        public string idAmm;
        public string periodicita;
        public string dataEsecuzione;
        public string giornoEsecuzione;
        public string meseEsecuzione;
        public bool arrivo = false;
        public bool partenza = false;
        public bool interno = false;
        public bool grigio = false;
        public string idTemplate;
        public string idStato;
        public string operatoreStato;
        public string idRegistro;
        public string idRF;
        public string idUO;
        public string UOsottoposte;
        public string idTitolario;
        public string tipoClassificazione;
        public string idFascicolo;
        public string digitali;
        public string firmati;
        public string marcati;
        public string scadenzaMarca;
        public string filtroDataCreazione;
        public string dataCreazioneDa;
        public string dataCreazioneA;
        public string filtrodataProtocollazione;
        public string dataProtocollazioneDa;
        public string dataProtocollazioneA;
        public string tipoRegistroStampa;
        public string idRepertorio;
        public string annoStampa;
        public string filtroDataStampa;
        public string dataStampaDa;
        public string dataStampaA;
        public Templates template = null;

        public string numGiorniCreazione;
        public string numGiorniProtocollazione;
        public string numGiorniStampa;

        public string statoVersamento;

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
