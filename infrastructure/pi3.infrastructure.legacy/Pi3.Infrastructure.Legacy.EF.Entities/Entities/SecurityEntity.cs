// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class SecurityEntity
    {
        public long? THING { get; set; }
        public long? PERSONORGROUP { get; set; }
        public long? ACCESSRIGHTS { get; set; }
        public long? ID_GRUPPO_TRASM { get; set; }
        public string? CHA_TIPO_DIRITTO { get; set; }
        public string? HIDE_DOC_VERSIONS { get; set; }
        public string? VAR_NOTE_SEC { get; set; }
        public string? CHA_COPIA_VISIBILITA { get; set; }
        public DateTime? TS_INSERIMENTO { get; set; }
    }
}
