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
    public class File
    {
        [XmlAttribute]
        public string Formato { get; set; }

        [XmlAttribute]
        public string Dimensione { get; set; }

        [XmlAttribute]
        public string Impronta { get; set; }

        [XmlAttribute]
        public string AlgoritmoHash { get; set; }
    }
}
