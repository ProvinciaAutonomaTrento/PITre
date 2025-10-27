// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.FormatiDocumento;
using DocsPaVO.ProfilazioneDinamica;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace DocsPaVO.Conservazione.PARER
{
    [Serializable()]
    [DataContract]
    public class PolicyPARER
    {
        [DataMember]
        public string id { get; set; }
        [DataMember]
        public string tipo { get; set; }
        [DataMember]
        public string codice { get; set; }
        [DataMember]
        public string descrizione { get; set; }
        [DataMember]
        public string isAttiva { get; set; }
        [DataMember]
        public string idGruppoRuoloResp { get; set; }
        [DataMember]
        public string idAmm { get; set; }
        [DataMember]
        public string periodicita { get; set; }
        [DataMember]
        public string dataEsecuzione { get; set; }
        [DataMember]
        public string giornoEsecuzione { get; set; }
        [DataMember]
        public string meseEsecuzione { get; set; }
        [DataMember]
        public bool arrivo { get; set; } = false;
        [DataMember]
        public bool partenza { get; set; } = false;
        [DataMember]
        public bool interno { get; set; } = false;
        [DataMember]
        public bool grigio { get; set; } = false;
        [DataMember]
        public string idTemplate { get; set; }
        [DataMember]
        public string idStato { get; set; }
        [DataMember]
        public string operatoreStato { get; set; }
        [DataMember]
        public string idRegistro { get; set; }
        [DataMember]
        public string idRF { get; set; }
        [DataMember]
        public string idUO { get; set; }
        [DataMember]
        public string UOsottoposte { get; set; }
        [DataMember]
        public string idTitolario { get; set; }
        [DataMember]
        public string tipoClassificazione { get; set; }
        [DataMember]
        public string idFascicolo { get; set; }
        [DataMember]
        public string digitali { get; set; }
        [DataMember]
        public string firmati { get; set; }
        [DataMember]
        public string marcati { get; set; }
        [DataMember]
        public string scadenzaMarca { get; set; }
        [DataMember]
        public string filtroDataCreazione { get; set; }
        [DataMember]
        public string dataCreazioneDa { get; set; }
        [DataMember]
        public string dataCreazioneA { get; set; }
        [DataMember]
        public string filtrodataProtocollazione { get; set; }
        [DataMember]
        public string dataProtocollazioneDa { get; set; }
        [DataMember]
        public string dataProtocollazioneA { get; set; }
        [DataMember]
        public string tipoRegistroStampa { get; set; }
        [DataMember]
        public string idRepertorio { get; set; }
        [DataMember]
        public string annoStampa { get; set; }
        [DataMember]
        public string filtroDataStampa { get; set; }
        [DataMember]
        public string dataStampaDa { get; set; }
        [DataMember]
        public string dataStampaA { get; set; }
        [DataMember]
        public Templates template { get; set; } = null;
        [DataMember]

        public string numGiorniCreazione { get; set; }
        [DataMember]
        public string numGiorniProtocollazione { get; set; }
        [DataMember]
        public string numGiorniStampa { get; set; }

        [DataMember]
        public string statoVersamento { get; set; }
        [DataMember]
        public string escludiFatture { get; set; }

        [DataMember]
        public string numGiorniFirma { get; set; }
        [DataMember]
        public string bigFiles { get; set; }

        [DataMember]
        public string ente { get; set; }
        [DataMember]
        public string struttura { get; set; }

        [DataMember]
        public string notificaMail { get; set; }

        [DataMember]
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
