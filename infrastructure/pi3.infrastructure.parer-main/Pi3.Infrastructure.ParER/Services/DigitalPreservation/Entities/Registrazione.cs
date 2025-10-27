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
    public class Registrazione
    {
        [XmlAttribute]
        public string? CodiceAOO { get; set; }
        [XmlAttribute]
        public string? DescrizioneAOO { get; set; }

        [XmlAttribute]
        public string? SegnaturaProtocollo { get; set; }

        [XmlAttribute]
        public string? NumeroProtocollo { get; set; }

        [XmlAttribute]
        public string? TipoProtocollo { get; set; }

        [XmlAttribute]
        public string? DataProtocollo { get; set; }

        [XmlAttribute]
        public string? OraProtocollo { get; set; }

        [XmlAttribute]
        public string? SegnaturaEmergenza { get; set; }

        [XmlAttribute]
        public string? NumeroProtocolloEmergenza { get; set; }

        [XmlAttribute]
        public string? DataProtocolloEmergenza { get; set; }

        public ProtocolloMittente? ProtocolloMittente { get; set; }

        public Utente? Protocollista { get; set; }

        public Corrispondente[] Mittente { get; set; }
        public Corrispondente[] Destinatario { get; set; }
    }

    public class ProtocolloMittente
    {
        [XmlAttribute]
        public string? Protocollo { get; set; }

        [XmlAttribute]
        public string? Data { get; set; }

        [XmlAttribute]
        public string? MezzoSpedizione { get; set; }
    }
}
