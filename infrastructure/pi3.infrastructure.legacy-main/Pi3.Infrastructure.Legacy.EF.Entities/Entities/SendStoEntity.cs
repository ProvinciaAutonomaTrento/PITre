// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class SendStoEntity
    {
        public long SYSTEM_ID { get; set; }

        public long? ID_CORR_GLOBALE { get; set; }

        public long? ID_PROFILE { get; set; }

        public DateTime? DTA_SPEDIZIONE { get; set; }

        public string? ESITO { get; set; }

        public string? MAIL { get; set; }

        public long? ID_DOCUMENTTYPE { get; set; }

        public long? ID_GROUP_SENDER { get; set; }

        public string? MAIL_MITTENTE { get; set; }

        public long? ID_REG_MAIL_MITTENTE { get; set; }
    }
}
