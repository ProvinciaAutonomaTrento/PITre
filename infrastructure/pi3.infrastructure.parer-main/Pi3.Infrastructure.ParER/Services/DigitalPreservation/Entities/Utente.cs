// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Pi3.Infrastructure.ParER.Services.DigitalPreservation.Entities
{
    public class Utente
    {
        [XmlAttribute]
        public string? CodiceUtente { get; set; }

        [XmlAttribute]
        public string? DescrizioneUtente { get; set; }

        [XmlAttribute]
        public string? CodiceRuolo { get; set; }

        [XmlAttribute]
        public string? DescrizioneRuolo { get; set; }

        [XmlAttribute]
        public string? UOAppartenenza { get; set; }
    }
}
