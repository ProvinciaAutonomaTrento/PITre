using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace DocsPaVO.Import.Pregressi
{
    [Serializable()]
    public class ReportPregressi
    {
        public string systemId = string.Empty;
        public string idAmm = string.Empty;
        public string idUtenteCreatore = string.Empty;
        public string idRuoloCreatore = string.Empty;
        public string dataEsecuzione = string.Empty;
        public string dataFine = string.Empty;
        public string numDoc = string.Empty;
        public string numeroElaborati = string.Empty;
        public string inError = string.Empty;
        //Andrea - parametro descrizione
        public string descrizione = string.Empty;

        [XmlArray()]
        [XmlArrayItem(typeof(ItemReportPregressi))]
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
