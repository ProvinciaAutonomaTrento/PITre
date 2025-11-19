// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class VersionEntity
    {
        public long? VERSION_ID { get; set; }
        public long? DOCNUMBER { get; set; }
        public int? VERSION { get; set; }
        public string? SUBVERSION { get; set; }
        public string? VERSION_LABEL { get; set; }
        public long? AUTHOR { get; set; }
        public long? TYPIST { get; set; }
        public DateTime? LASTEDITDATE { get; set; }
        public DateTime? LASTEDITTIME { get; set; }
        public string? COMMENTS { get; set; }
        public long? NUM_PAG_ALLEGATI { get; set; }
        public DateTime? DTA_CREAZIONE { get; set; }
        public string? CHA_DA_INVIARE { get; set; }
        public DateTime? DTA_ARRIVO { get; set; }
        public string? V_NAME_FN { get; set; }
        public long? CARTACEO { get; set; }
        public long? SCARTA_FASC_CARTACEA { get; set; }
        public long? ID_PEOPLE_DELEGATO { get; set; }
        public string? CHA_SEGNATURA { get; set; }
        public string? CHA_ALLEGATI_ESTERNO { get; set; }
    }
}
