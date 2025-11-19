// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class LdapConfigEntity
    {
        public long ID_AMM { get; set; }
        public string SERVER_NAME { get; set; }
        public string GROUP_DN { get; set; }
        public string? USER_NAME { get; set; }
        public string? PASSWORD { get; set; }
        public string? USERID_ATTRIBUTE { get; set; }
        public string? EMAIL_ATTRIBUTE { get; set; }
        public string? MATRICOLA_ATTRIBUTE { get; set; }
        public string? NOME_ATTRIBUTE { get; set; }
        public string? COGNOME_ATTRIBUTE { get; set; }
        public string? SEDE_ATTRIBUTE { get; set; }
        public string? ID_USER { get; set; }

    }
}
