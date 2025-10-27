// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.AggregateModels.DocumentAggregate.ValueObjects;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.DocumentAggregate.Events
{
    public class DocumentVersionAddedEvent : Event
    {
        public DocumentVersionAddedEvent()
        { }

        public string IdVersion { get; init; }

        public TextValue Name { get; init; }

        public int VersionNumber { get; init; }

        public DateTime CreationDate { get; init; }

        public DocumentBlobRef? DocumentBlobRef { get; init; } = null;

        public bool? DigitalSigned { get; init; } = null;

        public int? PageCount { get; init; } = null;

        public bool? FromPaper { get; init; } = null;
    }
}
