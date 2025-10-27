// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace DocsPaVO.Import.Pregressi
{
    [Serializable()]
    [DataContract]
    public class EsitoImportPregressi
    {
        public bool esito = false;

        [XmlArray()]
        [XmlArrayItem(typeof(ItemReportPregressi))]
        [DataMember]
        public List<ItemReportPregressi> itemPregressi { get; set; }

        public EsitoImportPregressi()
        { this.itemPregressi = new List<ItemReportPregressi>(); }
    }
}
