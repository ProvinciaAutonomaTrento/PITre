// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace DocsPaVO.RegistroAccessi.FOIA
{
    public class Oggetto
    {
        [XmlElement(ElementName = "sintesianonimizzataoggetto")]
        public String Sintesi { get; set; }

        [XmlElement(ElementName = "esito")]
        public Esito Esito { get; set; }

        [XmlElement(ElementName = "esitoRiesame")]
        public Esito EsitoRiesame { get; set; }

    }
}
