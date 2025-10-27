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
    public class SoggettoProduttore
    {
        public Amministrazione Amministrazione { get; set; }

        public GerarchiaUO[]? GerarchiaUO { get; set; }

        public Utente Creatore { get; set; }
    }

    public class Amministrazione
    {
        [XmlAttribute]
        public string? CodiceAmministrazione { get; set; }

        [XmlAttribute]
        public string? DescrizioneAmministrazione { get; set; }
    }

    public class GerarchiaUO
    {
        public UnitaOrganizzativa UnitàOrganizzativa { get; set; }
    }

    public class UnitaOrganizzativa
    {
        [XmlAttribute]
        public string? CodiceUO { get; set; }

        [XmlAttribute]
        public string? DescrizioneUO { get; set; }

        [XmlAttribute]
        public string? Livello { get; set; }
    }

}
