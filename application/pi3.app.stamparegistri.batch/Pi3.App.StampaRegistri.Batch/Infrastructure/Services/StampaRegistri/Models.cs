// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.StampaRegistri.Batch.Infrastructure.Services.StampaRegistri
{
    public class ReportRegistriItem : ValueObject
    {
        public long Docnumber { get; set; }

        public long? RecordNumber { get; set; }

        public DateTime? RecordDate { get; set; }

        public string? RecordType { get; set; }

        public DateTime? CancellationDate { get; set; }

        public string? EmergencyRecordNumber { get; set; }

        public string? Subject { get; set; }

        public string? SenderRecipients { get; set; }

        public string? Folders { get; set; }

        public string? Hash { get; set; }

        public long AttachmentsNumber { get; set; }

        public bool IsSubjectModified { get; set; }

        public bool IsSenderOrRecipientsModified { get; set; }

    }

    public class PrintRange : ValueObject
    {
        public long? Year { get; set; }

        public long? NumProtoStart { get; set; }

        public long? NumProtoEnd { get; set; }

    }
}
