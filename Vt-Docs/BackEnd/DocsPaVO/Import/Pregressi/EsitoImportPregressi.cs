using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace DocsPaVO.Import.Pregressi
{
    [Serializable()]
    public class EsitoImportPregressi
    {
        public bool esito = false;

        [XmlArray()]
        [XmlArrayItem(typeof(ItemReportPregressi))]
        public List<ItemReportPregressi> itemPregressi { get; set; }

        public EsitoImportPregressi()
        { this.itemPregressi = new List<ItemReportPregressi>(); }
    }
}
