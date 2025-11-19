// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Pi3.Core.AggregateModels.DocumentAggregate.ValueObjects;
using Pi3.Core.SeedWork;

namespace Pi3.Core.AggregateModels.DocumentAggregate.Entities
{
    public class DocumentVersion : Entity<string>
    {
        public DocumentVersion(string id, TextValue name,
                int versionNumber, DateTime creationDate,
                DocumentBlobRef? documentBlobRef = null,
                bool? digitalSigned = null,
                bool? fromPaper = null)
        {
            Id = id;
            Name = name;
            VersionNumber = versionNumber;
            CreationDate = creationDate;
            DigitalSigned = digitalSigned;
            AssignDocumentBlobRef(documentBlobRef);
            FromPaper = fromPaper;
        }

        public string Id { get; protected set; }
        public TextValue Name { get; protected set; }
        public int VersionNumber { get; protected set; }
        public DateTime CreationDate { get; protected set; }
        public DocumentBlobRef? DocumentBlobRef { get; protected set; }
        public bool? DigitalSigned { get; protected set; }
        public bool? FromPaper { get; protected set; }

        internal void ChangeName(TextValue? newName = null)
        {
            this.Name = newName;
        }

        internal void AssignDocumentBlobRef(DocumentBlobRef? documentBlobRef)
        {
            DocumentBlobRef = documentBlobRef;

            if (!DigitalSigned.HasValue)
            {
                if (documentBlobRef != null)
                {
                    switch (Path.GetExtension(documentBlobRef.FileName).ToLowerInvariant())
                    {
                        case ".p7m":
                        case ".m7m":
                        case ".tsr":
                            DigitalSigned = true;
                            break;
                        default:
                            DigitalSigned = false;
                            break;
                    }
                }
                else
                    DigitalSigned = false;
            }
        }
    }
}
