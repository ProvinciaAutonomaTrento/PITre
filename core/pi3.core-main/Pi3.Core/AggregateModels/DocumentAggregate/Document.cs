// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.AggregateModels.ContentElementAggregate;
using Pi3.Core.AggregateModels.ElementAggregate;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Pi3.Core.AggregateModels.DocumentAggregate
{
    public class Document : ContentElement
    {
        #region Public Members
        
        protected Document() : base()
        { }

        protected Document(string id, string idTenant, string typeName, DateTime creationDate, TextValue name, TextValue? description = null)
        : base(id, idTenant, typeName, creationDate, name, description)
        {
        }

        public Document(string id, string idTenant, DateTime creationDate, TextValue name, TextValue? description = null)
                : base(id, idTenant, "Document", creationDate, name, description)
        {
        }

        public Document(string idTenant, DateTime creationDate, TextValue name, TextValue? description = null)
            : base(null, idTenant, "Document", creationDate, name, description)
        {   
        }

        public virtual void ChangeIdParentDocument(string newIdParentDocument)
        {
            this.ApplyChange(new IdParentDocumentChangedEvent() { Id = this.Id, NewIdParentDocument = newIdParentDocument });
        }

        public string? IdParentDocument
        {
            get;
            protected set;
        }

        public DocumentVersion CurrentVersion
        {
            get
            {
                return _versions.OrderByDescending(v => v.Id).FirstOrDefault();
            }
        }

        public IReadOnlyList<DocumentVersion> Versions
        {
            get
            {
                return _versions.AsReadOnly();
            }
        }

        public virtual void AddVersion(string id, TextValue name, int versionNumber, DateTime creationDate, DocumentBlobRef? documentBlobRef = null, bool? digitalSigned = null)
        {
            ApplyChange(new DocumentVersionAdded()
            {
                Id = this.Id,
                IdVersion = id,
                Name = name,
                VersionNumber = versionNumber,
                CreationDate = creationDate,
                DocumentBlobRef = documentBlobRef,
                DigitalSigned = digitalSigned
            });
        }

        public virtual void RemoveVersion(string id)
        {
            if (!this._versions.Any(v => v.Id == id))
                throw new VersionNotFoundPi3Exception(Id);

            ApplyChange(new DocumentVersionRemoved()
            {
                Id = this.Id,
                IdVersion = id
            });
        }

        public virtual void AssignDocumentBlobRef(DocumentBlobRef documentBlobRef, TargetVersionBehavior targetVersionBehavior)
        {
            Validator.ValidateObject(documentBlobRef, new ValidationContext(documentBlobRef), true);
            Validator.ValidateObject(targetVersionBehavior, new ValidationContext(targetVersionBehavior), true);

            if (this.GetUncommittedChanges().Count(e => e.GetType() == typeof(DocumentBlobRefAssigned)) > 0)
                throw new NotSupportedPi3Exception(ErrorDescriptions.FileAlreadyUploaded, ErrorDescriptions.ResourceManager);
            
            if (!targetVersionBehavior.CreateNewVersion)
            {
                if (!this._versions.Any(v => v.Id == targetVersionBehavior.IdVersion))
                    throw new VersionNotFoundPi3Exception(targetVersionBehavior.IdVersion);
            }

            ApplyChange(new DocumentBlobRefAssigned()
            {
                Id = this.Id,
                DocumentBlobRef = documentBlobRef,
                TargetVersionBehavior = targetVersionBehavior
            });
        }

        public virtual void Recycle()
        {
            if (this.InRecycleBin)
                throw new NotSupportedPi3Exception(ErrorDescriptions.DocumentAlreadyDeleted, ErrorDescriptions.ResourceManager);

            this.ApplyChange(new DocumentAddedInRecycleBinEvent()
            {
                Id = this.Id
            });
        }

        public bool InRecycleBin 
        { 
            get; 
            protected set; 
        }

        public virtual void Delete()
        {
            if (this.Deleted)
                throw new NotSupportedPi3Exception(ErrorDescriptions.DocumentAlreadyDeleted, ErrorDescriptions.ResourceManager);

            this.ApplyChange(new DocumentDeletedEvent()
            {
                Id = this.Id
            });
        }

        public bool Deleted
        {
            get;
            protected set;
        }

        #endregion

        #region Private Members

        protected List<DocumentVersion> _versions;

        protected override void Handle(ElementCreatedEvent @event)
        {
            base.Handle(@event);

            this._versions = new List<DocumentVersion>();
        }

        protected virtual void Handle(IdParentDocumentChangedEvent @event)
        {
            this.IdParentDocument = @event.NewIdParentDocument;
        }

        protected virtual void Handle(DocumentVersionAdded @event)
        {
            _versions.Add(new DocumentVersion(@event.IdVersion, @event.Name, @event.VersionNumber, @event.CreationDate, @event.DocumentBlobRef, @event.DigitalSigned));
        }

        protected virtual void Handle(DocumentVersionRemoved @event)
        {
            _versions.RemoveAll(v => v.Id == @event.IdVersion);
        }

        protected virtual void Handle(DocumentBlobRefAssigned @event)
        {
            DocumentVersion version = null;

            if (!@event.TargetVersionBehavior.CreateNewVersion)
            {
                version = this._versions.First(v => v.Id == @event.TargetVersionBehavior.IdVersion);
            
                version.AssignDocumentBlobRef(@event.DocumentBlobRef);
            }
        }

        protected virtual void Handle(DocumentAddedInRecycleBinEvent @event)
        {
            this.InRecycleBin = true;
        }

        protected virtual void Handle(DocumentDeletedEvent @event)
        {
            this.Deleted = true;
        }

        #endregion
    }
}