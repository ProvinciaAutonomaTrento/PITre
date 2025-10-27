// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace DocsPaVO.Conservazione.PARER.BigFiles
{
    [XmlRoot(ElementName = "getStatoOggetto")]
    public class RecuperoStatoOggetto
    {
        [XmlElement(ElementName = "nmAmbiente")]
        public string Ambiente { get; set; }

        [XmlElement(ElementName = "nmVersatore")]
        public string Versatore { get; set; }

        [XmlElement(ElementName = "cdKeyObject")]
        public string Chiave { get; set; }
    }
}
