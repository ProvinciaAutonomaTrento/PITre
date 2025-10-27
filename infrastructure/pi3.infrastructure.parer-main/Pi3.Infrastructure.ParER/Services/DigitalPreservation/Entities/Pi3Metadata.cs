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
    public class Documento
    {
        [XmlAttribute]
        public string IdDocumento { get; set; }

        [XmlAttribute]
        public DateTime DataCreazione { get; set; }

        [XmlAttribute]
        public string Oggetto { get; set; }

        [XmlAttribute]
        public string Tipo { get; set; }

        [XmlAttribute]
        public string? LivelloRiservatezza { get; set; }

        public SoggettoProduttore? SoggettoProduttore { get; set; }

        public Registrazione? Registrazione { get; set; }

        public ContestoArchivistico? ContestoArchivistico { get; set; }

        public Tipologia? Tipologia { get; set; }

        public Allegato[]? Allegati { get; set; }

        public File? File { get; set; }
    }
}
