// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;
using DocsPaVO.FormatiDocumento;
using DocsPaVO.ProfilazioneDinamica;

namespace DocsPaVO.Conservazione
{
    [Serializable()]
    [DataContract]
    public class Policy
    {
        [DataMember]
        public string system_id { get; set; } = string.Empty;
        [DataMember]
        public string tipo { get; set; } = string.Empty;
        [DataMember]
        public string nome { get; set; } = string.Empty;
        [DataMember]
        public string idTemplate { get; set; } = string.Empty;
        [DataMember]
        public string idStatoDiagramma { get; set; } = string.Empty;
        [DataMember]
        public string idRf { get; set; } = string.Empty;
        [DataMember]
        public string classificazione { get; set; } = string.Empty;
        [DataMember]
        public string idAmministrazione { get; set; } = string.Empty;
        [DataMember]
        public bool arrivo { get; set; } = false;
        [DataMember]
        public bool partenza { get; set; } = false;
        [DataMember]
        public bool interno { get; set; } = false;
        [DataMember]
        public bool grigio { get; set; } = false;
        [DataMember]
        public bool automatico { get; set; } = false;
        [DataMember]
        public bool consolidazione { get; set; } = false;
        [DataMember]
        public string idAOO { get; set; } = string.Empty;
        [DataMember]
        public string dataCreazioneDa { get; set; } = string.Empty;
        [DataMember]
        public string dataCreazioneA { get; set; } = string.Empty;
        [DataMember]
        public string dataProtocollazioneDa { get; set; } = string.Empty;
        [DataMember]
        public string dataProtocollazioneA { get; set; } = string.Empty;
        [DataMember]
        public string tipoPeriodo { get; set; } = string.Empty;
        [DataMember]
        public string periodoGiornalieroNGiorni { get; set; } = string.Empty;
        [DataMember]
        public string periodoGiornalieroOre { get; set; } = string.Empty;
        [DataMember]
        public string periodoGiornalieroMinuti { get; set; } = string.Empty;
        [DataMember]
        public bool periodoSettimanaleLunedi { get; set; } = false;
        [DataMember]
        public bool periodoSettimanaleMartedi { get; set; } = false;
        [DataMember]
        public bool periodoSettimanaleMercoledi { get; set; } = false;
        [DataMember]
        public bool periodoSettimanaleGiovedi { get; set; } = false;
        [DataMember]
        public bool periodoSettimanaleVenerdi { get; set; } = false;
        [DataMember]
        public bool periodoSettimanaleSabato { get; set; } = false;
        [DataMember]
        public bool periodoSettimanaleDomenica { get; set; } = false;
        [DataMember]
        public string periodoSettimanaleOre { get; set; } = string.Empty;
        [DataMember]
        public string periodoSettimanaleMinuti { get; set; } = string.Empty;
        [DataMember]
        public string periodoMensileGiorni { get; set; } = string.Empty;
        [DataMember]
        public string periodoMensileOre { get; set; } = string.Empty;
        [DataMember]
        public string periodoMensileMinuti { get; set; } = string.Empty;
        [DataMember]
        public string idRuolo { get; set; } = string.Empty;
        [DataMember]
        public bool periodoAttivo { get; set; } = false;
        [DataMember]
        public string avvisoMesi { get; set; } = string.Empty;
        // MEV CS 1.5
        // scadenza verifiche leggibilità supporti
        [DataMember]
        public string avvisoMesiLegg { get; set; } = string.Empty;
        // fine MEV CS 1.5
        [DataMember]
        public string idUtenteRuolo { get; set; } = string.Empty;
        [DataMember]
        public string idGruppo { get; set; } = string.Empty;
        [DataMember]
        public string codiceUtente { get; set; } = string.Empty;
        [DataMember]
        public string tipoClassificazione { get; set; } = string.Empty;
        [DataMember]
        public string idUoCreatore { get; set; } = string.Empty;
        [DataMember]
        public string tipoDataCreazione { get; set; } = string.Empty;
        [DataMember]
        public string tipoDataProtocollazione { get; set; } = string.Empty;
        [DataMember]
        public bool uoSottoposte { get; set; } = false;
        [DataMember]
        public bool statoInviato { get; set; } = false;
        // MEV CS 1.5 F02_01
        // scadenza verifiche leggibilità supporti
        [DataMember]
        public bool statoConversione { get; set; } = false;
        // fine MEV CS 1.5 F02_01
        [DataMember]
        public bool includiSottoNodi { get; set; } = false;
        [DataMember]
        public bool soloDigitali { get; set; } = false;
        [DataMember]
        public bool soloFirmati { get; set; } = false;
        [DataMember]
        public string periodoAnnualeGiorno { get; set; } = string.Empty;
        [DataMember]
        public string periodoAnnualeMese { get; set; } = string.Empty;
        [DataMember]
        public string periodoAnnualeOre { get; set; } = string.Empty;
        [DataMember]
        public string periodoAnnualeMinuti { get; set; } = string.Empty;
        [DataMember]
        public string tipoConservazione { get; set; } = string.Empty;
        [DataMember]
        public Templates template { get; set; }= null;

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
