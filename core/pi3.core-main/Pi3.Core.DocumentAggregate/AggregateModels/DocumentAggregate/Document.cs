// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.AggregateModels.ContentElementAggregate;
using Pi3.Core.AggregateModels.DocumentAggregate.Entities;
using Pi3.Core.AggregateModels.DocumentAggregate.Events;
using Pi3.Core.AggregateModels.DocumentAggregate.Exceptions;
using Pi3.Core.AggregateModels.DocumentAggregate.Resources;
using Pi3.Core.AggregateModels.DocumentAggregate.ValueObjects;
using Pi3.Core.AggregateModels.ElementAggregate.Events;
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

        public virtual void AddVersion(string id, TextValue name, int versionNumber, DateTime creationDate, DocumentBlobRef? documentBlobRef = null, bool? digitalSigned = null, int? pageCount = null, bool? fromPaper = null)
        {
            ApplyChange(new DocumentVersionAddedEvent()
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

        public virtual void CreateEmptyVersion(TextValue? name = null)
        {
            ApplyChange(new DocumentVersionEmptyCreatedEvent()
            {
                Name = name
            });
        }

        public virtual void ChangeVersionName(string id, TextValue? newName = null)
        {
            if (!this._versions.Any(v => v.Id == id))
                throw new VersionNotFoundPi3Exception(Id);

            ApplyChange(new DocumentVersionNameChangedEvent()
            {
                IdVersion = id,
                NewName = newName
            });
        }

        public virtual void RemoveVersion(string id)
        {
            if (!this._versions.Any(v => v.Id == id))
                throw new VersionNotFoundPi3Exception(Id);

            ApplyChange(new DocumentVersionRemovedEvent()
            {
                Id = this.Id,
                IdVersion = id
            });
        }

        public virtual void AssignDocumentBlobRef(DocumentBlobRef documentBlobRef, TargetVersionBehavior targetVersionBehavior)
        {
            Validator.ValidateObject(documentBlobRef, new ValidationContext(documentBlobRef), true);
            Validator.ValidateObject(targetVersionBehavior, new ValidationContext(targetVersionBehavior), true);

            if (this.GetUncommittedChanges().Count(e => e.GetType() == typeof(DocumentBlobRefAssignedEvent)) > 0)
                throw new NotSupportedPi3Exception(ErrorDescriptions.FileAlreadyUploaded, ErrorDescriptions.ResourceManager);
            
            if (!targetVersionBehavior.CreateNewVersion)
            {
                if (!this._versions.Any(v => v.Id == targetVersionBehavior.IdVersion))
                    throw new VersionNotFoundPi3Exception(targetVersionBehavior.IdVersion);
            }

            ApplyChange(new DocumentBlobRefAssignedEvent()
            {
                Id = this.Id,
                DocumentBlobRef = documentBlobRef,
                TargetVersionBehavior = targetVersionBehavior
            });
        }

        public virtual void Recycle(TextValue note)
        {
            if (this.InRecycleBin)
                throw new NotSupportedPi3Exception(ErrorDescriptions.DocumentAlreadyDeleted, ErrorDescriptions.ResourceManager);

            this.ApplyChange(new DocumentAddedInRecycleBinEvent()
            {
                Id = this.Id,
                Note = note
            });
        }
        
        public virtual void Restore()
        {
            if (!this.InRecycleBin)
                throw new NotSupportedPi3Exception(ErrorDescriptions.DocumentNotInRecycleBin, ErrorDescriptions.ResourceManager);

            this.ApplyChange(new DocumentRestoredEvent()
            {
                Id = this.Id,
                Note = new TextValue(string.Empty)
            }); 
        }

        public bool InRecycleBin 
        { 
            get; 
            protected set; 
        }

        public TextValue NoteRecycleBin
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

        protected virtual void Handle(DocumentVersionAddedEvent @event)
        {
            _versions.Add(new DocumentVersion(@event.IdVersion, @event.Name, @event.VersionNumber, @event.CreationDate, @event.DocumentBlobRef, @event.DigitalSigned, @event.FromPaper));
        }

        protected virtual void Handle(DocumentVersionEmptyCreatedEvent @event)
        {
        }

        protected virtual void Handle(DocumentVersionNameChangedEvent @event)
        {
            _versions.Find(v => v.Id == @event.IdVersion)?.ChangeName(@event.NewName);
        }

        protected virtual void Handle(DocumentVersionRemovedEvent @event)
        {
            _versions.RemoveAll(v => v.Id == @event.IdVersion);
        }

        protected virtual void Handle(DocumentBlobRefAssignedEvent @event)
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
            this.NoteRecycleBin = @event.Note;
        }
        
        protected virtual void Handle(DocumentRestoredEvent @event)
        {
            this.InRecycleBin = false;
            this.NoteRecycleBin = new TextValue(string.Empty);
        }

        protected virtual void Handle(DocumentDeletedEvent @event)
        {
            this.Deleted = true;
        }

        #endregion
    }
}