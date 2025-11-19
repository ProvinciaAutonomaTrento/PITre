// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
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
