// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class ConsolidatedDocsEntity
    {
        public long? ID { get; set; }

        public string? DOCNAME { get; set; }

        public DateTime? CREATION_DATE { get; set; }

        public int? DOCUMENTTYPE { get; set; }

        public int? AUTHOR { get; set; }

        public string? AUTHOR_NAME { get; set; }

        public long? ID_RUOLO_CREATORE { get; set; }

        public string? RUOLO_CREATORE { get; set; }

        public int? NUM_PROTO { get; set; }

        public int? NUM_ANNO_PROTO { get; set; }

        public DateTime? DTA_PROTO { get; set; }

        public long? ID_PEOPLE_PROT { get; set; }

        public string? PEOPLE_PROT { get; set; }

        public long? ID_RUOLO_PROT { get; set; }

        public string? RUOLO_PROT { get; set; }

        public long? ID_REGISTRO { get; set; }

        public string? REGISTRO { get; set; }

        public string? CHA_TIPO_PROTO { get; set; }

        public string? VAR_PROTO_IN { get; set; }

        public DateTime? DTA_PROTO_IN { get; set; }

        public DateTime? DTA_ANNULLA { get; set; }

        public long? ID_OGGETTO { get; set; }

        public string? VAR_PROF_OGGETTO { get; set; }

        public string? MITT_DEST { get; set; }

        public long? ID_DOCUMENTO_PRINCIPALE { get; set; }
    }
}
