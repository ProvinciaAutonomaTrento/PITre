// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class NotifyEntity
    {
        public string? DESC_PRODUCER { get; set; }
        public string? FIELD_1 { get; set; }
        public string? FIELD_2 { get; set; }
        public string? FIELD_3 { get; set; }
        public string? FIELD_4 { get; set; }
        public string? SPECIALIZED_FIELD { get; set; }
        public string? TYPE_EVENT { get; set; }
        public string? NOTES { get; set; }
        public string? COLOR { get; set; }
        public string? DOMAINOBJECT { get; set; }
        public long SYSTEM_ID { get; set; }
        public long ID_EVENT { get; set; }
        public long ID_PEOPLE_RECEIVER { get; set; }
        public long? ID_GROUP_RECEIVER { get; set; }
        public long? ID_OBJECT { get; set; }
        public long? ID_SPECIALIZED_OBJECT { get; set; }
        public DateTime DTA_NOTIFY { get; set; }
        public DateTime DTA_EVENT { get; set; }
        public string? MULTIPLICITY { get; set; }
        public string TYPE_NOTIFY { get; set; }
        public string? READ_NOTIFICATION { get; set; }
    }
}
