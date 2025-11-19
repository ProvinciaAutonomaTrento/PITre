// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace DocsPaVO.Import.Pregressi
{
    [Serializable()]
    [DataContract]
    public class ReportPregressi
    {
        [DataMember]
        public string systemId { get; set; } = string.Empty;
        [DataMember]
        public string idAmm { get; set; } = string.Empty;
        [DataMember]
        public string idUtenteCreatore { get; set; } = string.Empty;
        [DataMember]
        public string idRuoloCreatore { get; set; } = string.Empty;
        [DataMember]
        public string dataEsecuzione { get; set; } = string.Empty;
        [DataMember]
        public string dataFine { get; set; } = string.Empty;
        [DataMember]
        public string numDoc { get; set; } = string.Empty;
        [DataMember]
        public string numeroElaborati { get; set; } = string.Empty;
        [DataMember]
        public string inError { get; set; } = string.Empty;
        //Andrea - parametro descrizione
        [DataMember]
        public string descrizione { get; set; } = string.Empty;

        [XmlArray()]
        [XmlArrayItem(typeof(ItemReportPregressi))]
        [DataMember]
        public List<ItemReportPregressi> itemPregressi { get; set; }

        public ReportPregressi()
        {
            this.itemPregressi = new List<ItemReportPregressi>();
        }

        /// <summary>
        /// Funzione per l'aggiunta di un ItemReportPregressi alla lista
        /// Il campo verrà aggiunto solo se non ne esiste già uno uguale
        /// </summary>
        /// <param name="fieldSettings"></param>
        public void AddItemReportPregressi(ItemReportPregressi itemReportPreg)
        {
            if (!this.itemPregressi.Contains(itemReportPreg))
                this.itemPregressi.Add(itemReportPreg);
        }

        /// <summary>
        /// Funzione per la rimozione di un ItemReportPregressi dalla lista
        /// Il campo verrà eliminato solo se esiste nella lista
        /// </summary>
        /// <param name="fieldSettings"></param>
        public void DeleteItemReportPregressi(ItemReportPregressi itemReportPreg)
        {
            if (this.itemPregressi.Contains(itemReportPreg))
                this.itemPregressi.Remove(itemReportPreg);
        }
    }
}
