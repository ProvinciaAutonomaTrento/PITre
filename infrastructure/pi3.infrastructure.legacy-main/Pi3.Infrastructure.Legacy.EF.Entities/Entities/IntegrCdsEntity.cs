// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class IntegrCdsEntity
    {
        public DateTime? NOTIFY_DATE { get; set; }
        public long SYSTEM_ID { get; set; }
        public long? ID_FASC { get; set; }
        public long? ID_OGGETTO_LOG { get; set; }
        public long? IDDOC { get; set; }
        public long? IDALL { get; set; }
        public long? IDLOG { get; set; }
        public string? COD_APPLICANT { get; set; }
        public string? COD_LOCAT { get; set; }
        public string? DESC_FASC { get; set; }
        public string? DESC_LOG { get; set; }
        public string? SIGNATURE { get; set; }
        public string? DESC_APPLICANT { get; set; }
        public string? DESC_LOCAT { get; set; }
        public string? NOTIFYTYPE_LOG { get; set; }
        public string? NOTIFYTYPE_CDS { get; set; }
        public string? NOTIFY_RESULT { get; set; }
        public string? USERID { get; set; }
        public string? COD_FASC { get; set; }
        public string? APPLICANT { get; set; }
        public string? LOCAT { get; set; }

    }
}
