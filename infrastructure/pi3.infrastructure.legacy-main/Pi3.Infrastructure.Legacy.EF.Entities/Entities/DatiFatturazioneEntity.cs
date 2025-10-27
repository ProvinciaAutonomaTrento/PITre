// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class DatiFatturazioneEntity
    {
        public long SYSTEM_ID { get; set; }
        public string CODICE_AMM { get; set; }
        public string CODICE_AOO { get; set; }
        public string CODICE_UO { get; set; }
        public string? CODICE_UAC { get; set; }
        public string? CODICE_CLASSIFICAZIONE { get; set; }
        public string? VAR_UTENTE_PROPRIETARIO { get; set; }
        public string? VAR_TIPOLOGIA_DOCUMENTO { get; set; }
        public string? VAR_RAGIONE_TRASMISSIONE { get; set; }
    }
}
