// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocsPaVO.DocumentMetadata
{
    public class EventDocument
    {
        public string SystemId { get; set; }
        public string IdPeopleOperatore { get; set; }
        public string IdGruppoOperatore { get; set; }
        public string IdAmministrazione { get; set; }
        public string Oggetto { get; set; }
        public string IdOggetto { get; set; }
        public string DescrizioneOggetto { get; set; }
        public string CodiceAzione { get; set; }
        public string DescrizioneAzione { get; set; }
        public string IdTrasmissione { get; set; }
        public string DataAzione { get; set; }
        public string IdPeopleDelegante { get; set; }

    }
}
