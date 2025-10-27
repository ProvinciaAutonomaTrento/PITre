// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.AggregateModels.ElementAggregate;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.DocumentAggregate
{
    public class IdParentDocumentChangedEvent : Event
    {
        public IdParentDocumentChangedEvent()
        { }

        public string NewIdParentDocument { get; init; }
    }

    public class DocumentVersionAdded : Event
    {
        public DocumentVersionAdded()
        { }

        public string IdVersion { get; init; }

        public TextValue Name { get; init; }

        public int VersionNumber { get; init; }

        public DateTime CreationDate { get; init; }

        public DocumentBlobRef? DocumentBlobRef { get; init; } = null;

        public bool? DigitalSigned { get; init; } = null;
    }

    public class DocumentVersionRemoved : Event
    {
        public DocumentVersionRemoved()
        { }

        public string IdVersion { get; init; }
    }

    public class DocumentBlobRefAssigned : Event
    {
        public DocumentBlobRefAssigned()
        { }

        public DocumentBlobRef DocumentBlobRef { get; init; }

        public TargetVersionBehavior TargetVersionBehavior { get; init; }
    }

    public class DocumentAddedInRecycleBinEvent : Event
    {
        public DocumentAddedInRecycleBinEvent()
        { }
    }

    public class DocumentDeletedEvent : Event
    {
        public DocumentDeletedEvent()
        { }
    }
}
