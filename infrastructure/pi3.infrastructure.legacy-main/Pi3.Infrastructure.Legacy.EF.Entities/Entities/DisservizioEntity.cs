// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class DisservizioEntity
    {
        public long SYSTEM_ID { get; set; }
        public string STATO { get; set; }
        public string TESTO_NOTIFICA { get; set; }
        public string TESTO_EMAIL_NOTIFICA { get; set; }
        public string TESTO_PAG_CORTESIA { get; set; }
        public string TESTO_EMAIL_RIPRESA { get; set; }
        public long NOTIFICATO { get; set; }
    }
}
