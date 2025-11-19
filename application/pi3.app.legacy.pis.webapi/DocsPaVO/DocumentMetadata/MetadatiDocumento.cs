// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocsPaVO.DocumentMetadata
{
    public class MetadatiDocumento
    {
        public string SystemId { get; set; }
        public string IdProfile { get; set; }
        public string IdVersion { get; set; }
        public string MetadatiXML { get; set; }
        public string DataInserimento { get; set; }
        public string DataAzione { get; set; }
        public string CodiceAzione { get; set; }
        public string DescrizioneAzione { get; set; }
        public string DescrizioneOggetto { get; set; }
    }
}
