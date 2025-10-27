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
    public class Allegato
    {
        [XmlAttribute]
        public string ID { get; set; }

        [XmlAttribute]
        public string Descrizione { get; set; }

        [XmlAttribute]
        public string Tipo { get; set; }

        public File? File { get; set; }

    }
}
