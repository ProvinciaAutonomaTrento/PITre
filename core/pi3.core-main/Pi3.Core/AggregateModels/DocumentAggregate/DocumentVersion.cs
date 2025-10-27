// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.File.TextExtractors;

namespace Pi3.Core.AggregateModels.DocumentAggregate
{
    public class DocumentVersion : Entity<string>
    {
        public DocumentVersion(string id, TextValue name, 
                int versionNumber, DateTime creationDate, 
                DocumentBlobRef? documentBlobRef = null,
                bool? digitalSigned = null)
        {
            this.Id = id;
            this.Name = name;
            this.VersionNumber = versionNumber;
            this.CreationDate = creationDate;
            this.DigitalSigned = digitalSigned;
            this.AssignDocumentBlobRef(documentBlobRef);
        }

        public string Id { get; protected set; }
        public TextValue Name { get; protected set; }
        public int VersionNumber { get; protected set; }
        public DateTime CreationDate { get; protected set; }
        public DocumentBlobRef? DocumentBlobRef { get; protected set; }
        public bool? DigitalSigned { get; protected set; }

        internal void AssignDocumentBlobRef(DocumentBlobRef? documentBlobRef)
        {
            this.DocumentBlobRef = documentBlobRef;

            if (!this.DigitalSigned.HasValue)
            {
                if (documentBlobRef != null)
                {
                    switch (Path.GetExtension(documentBlobRef.FileName).ToLowerInvariant())
                    {
                        case ".p7m":
                        case ".m7m":
                        case ".tsr":
                            this.DigitalSigned = true;
                            break;
                        default:
                            this.DigitalSigned = false;
                            break;
                    }
                }
                else
                    this.DigitalSigned = false;
            }
        }
    }
}
