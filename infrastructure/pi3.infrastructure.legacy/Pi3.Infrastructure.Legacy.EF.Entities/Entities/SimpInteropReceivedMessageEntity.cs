// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class SimpInteropReceivedMessageEntity
    {
        public DateTime? RECEIVEDDATE { get; set; }
        public DateTime? RECORDDATE { get; set; }
        public long? RECORDNUMBER { get; set; }
        public long? PROFILEID { get; set; }
        public long? RECEIVEDPRIVATE { get; set; }
        public string? MESSAGEID { get; set; }
        public string? SENDERURL { get; set; }
        public string? SENDERADMINISTRATIONCODE { get; set; }
        public string? AOOCODE { get; set; }
        public string? RECEIVERCODE { get; set; }
        public string? SUBJECT { get; set; }
        public string? SENDERDESCRIPTION { get; set; }
    }
}
