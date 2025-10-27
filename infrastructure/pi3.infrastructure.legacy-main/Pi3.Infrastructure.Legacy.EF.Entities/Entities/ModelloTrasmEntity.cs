// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class ModelloTrasmEntity
    {
        public string? CHA_CEDE_DIRITTI { get; set; }
        public string? CHA_MANTIENI_LETTURA { get; set; }
        public string? CHA_MANTIENI_SCRITTURA { get; set; }
        public long SYSTEM_ID { get; set; }
        public long? ID_AMM { get; set; }
        public long? ID_REGISTRO { get; set; }
        public long? ID_PEOPLE { get; set; }
        public long? ID_PEOPLE_NEW_OWNER { get; set; }
        public long? ID_GROUP_NEW_OWNER { get; set; }
        public string? CHA_TIPO_OGGETTO { get; set; }
        public string? SINGLE { get; set; }
        public string? NO_NOTIFY { get; set; }
        public string? CODICE { get; set; }
        public string? VAR_NOTE_GENERALI { get; set; }
        public string? NOME { get; set; }
    }
}
