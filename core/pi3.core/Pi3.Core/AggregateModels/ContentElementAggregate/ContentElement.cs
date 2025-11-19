// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Pi3.Core.AggregateModels.ElementAggregate;
using Pi3.Core.SeedWork;

namespace Pi3.Core.AggregateModels.ContentElementAggregate
{
    public abstract class ContentElement : ElementAggregate.Element
    {
        #region Public Members

        protected ContentElement() : base()
        { }

        protected ContentElement(string id, string idTenant, string typeName, DateTime creationDate, TextValue name, TextValue? description = null)
            : base(id, idTenant, typeName, creationDate, name, description)
        {
        }

        protected ContentElement(string idTenant, string typeName, DateTime creationDate, TextValue name, TextValue? description = null)
            : base(idTenant, typeName, creationDate, name, description)
        {
        }

        public bool Reserved
        {
            get;
            protected set;
        }

        public DateTime? ReservedDate
        {
            get;
            protected set;
        }

        public string ReserveIdUser
        {
            get;
            protected set;
        }

        public virtual void Reserve(DateTime? reserveDate = null, string reserveUserId = null)
        {
            this.ApplyChange(new ContentElementReservedEvent()
            {
                Id = this.Id,
                ReservedDate = reserveDate ?? DateTime.Now,
                ReserveIdUser = reserveUserId
            });
        }

        public virtual void Unreserve()
        {
            this.ApplyChange(new ContentElementUnreservedEvent() { Id = this.Id });
        }

        public IReadOnlyList<ContentElementPermission> Permissions
        {
            get
            {
                return _permissions.AsReadOnly();
            }
        }

        public virtual void SetPublicPermission(ContentElementRightTypesEnum rightType, bool applyToChilds = false)
        {
            ApplyChange(new PublicPermissionSettedEvent() { Id = this.Id, RightType = rightType, ApplyToChilds = applyToChilds });
        }

        public virtual void RemovePublicPermission()
        {
            ApplyChange(new PublicPermissionRemovedEvent() { Id = this.Id });
        }

        public virtual void SetOwnerPermission(string idMember, string memberName, ContentElementMemberTypesEnum memberType, ContentElementRightTypesEnum rightType, bool applyToChilds = false)
        {
            ApplyChange(new OwnerPermissionSettedEvent() { Id = Id, IdMember = idMember, MemberName = memberName, MemberType = memberType, RightType = rightType, ApplyToChilds = applyToChilds });
        }

        public virtual void RemoveOwnerPermission(string idMember, string memberName, ContentElementMemberTypesEnum memberType)
        {
            ApplyChange(new OwnerPermissionRemovedEvent() { Id = this.Id, IdMember = idMember, MemberName = memberName, MemberType = memberType });
        }

        public virtual void SetMemberPermission(string idMember, string memberName, ContentElementMemberTypesEnum memberType, ContentElementRightTypesEnum rightType, bool applyToChilds = false)
        {
            ApplyChange(new MemberPermissionSettedEvent() { Id = this.Id, IdMember = idMember, MemberName = memberName, MemberType = memberType, RightType = rightType, ApplyToChilds = applyToChilds });
        }

        public virtual void RemoveMemberPermission(string idMember, string memberName, ContentElementMemberTypesEnum memberType)
        {
            ApplyChange(new MemberPermissionRemovedEvent() { Id = this.Id, IdMember = idMember, MemberName = memberName, MemberType = memberType });
        }

        public IReadOnlyList<RelatedContentElement> RelatedElements
        {
            get
            {
                return _relatedElements.AsReadOnly();
            }
        }

        public virtual void AddRelatedElement(string id, bool addAsParent = true)
        {
            ApplyChange(new RelatedContentElementAddedEvent() { Id = this.Id, IdRelatedElement = id, AsParent = addAsParent });
        }

        public virtual void RemoveReleatedElement(string id)
        {
            if (_relatedElements.Any(r => r.Id.Equals(id)))
                ApplyChange(new RelatedContentElementRemovedEvent() { Id = this.Id, IdRelatedElement = id});
        }

        public IReadOnlyList<ContentElementClassification> Classifications
        {
            get
            {
                return _classifications;
            }
        }

        public virtual void AddClassification(string id, TextValue? name = null)
        {
            ApplyChange(new ContentElementClassificationAddedEvent() { Id = this.Id, IdClassification = id, Name = name});
        }

        public virtual void RemoveClassification(string id)
        {
            if (_classifications.Any(c => c.Id.Equals(id)))
                ApplyChange(new ContentElementClassificationRemovedEvent() { Id = this.Id, IdClassification = id });
        }

        #endregion

        #region Private Members

        protected List<RelatedContentElement> _relatedElements;
        protected List<ContentElementClassification> _classifications;
        protected List<ContentElementPermission> _permissions;

        protected override void Handle(ElementCreatedEvent @event)
        {
            base.Handle(@event);

            this._permissions = new List<ContentElementPermission>();
            this._relatedElements = new List<RelatedContentElement>();
            this._classifications = new List<ContentElementClassification>();
        }

        protected virtual void Handle(ContentElementClassificationAddedEvent @event)
        {
            _classifications.Add(new ContentElementClassification(@event.IdClassification, @event.Name));
        }

        protected virtual void Handle(ContentElementClassificationRemovedEvent @event)
        {
            _classifications.RemoveAll(e => e.Id == @event.Id);
        }

        protected virtual void Handle(RelatedContentElementAddedEvent @event)
        {
            _relatedElements.Add(new RelatedContentElement(@event.Id, @event.AsParent));
        }

        protected virtual void Handle(RelatedContentElementRemovedEvent @event)
        {
            _relatedElements.RemoveAll(e => e.Id == @event.IdRelatedElement);
        }
        
        protected virtual void Handle(PublicPermissionSettedEvent @event)
        {
            _permissions.Add(new ContentElementPermission(null, null, null, @event.RightType, ContentElementPermissionTypesEnum.Public));
        }

        protected virtual void Handle(PublicPermissionRemovedEvent @event)
        {
            _permissions.RemoveAll(p => p.PermissionType == ContentElementPermissionTypesEnum.Public);
        }

        protected virtual void Handle(OwnerPermissionSettedEvent @event)
        {
            _permissions.RemoveAll(p => p.PermissionType == ContentElementPermissionTypesEnum.Owner);
            _permissions.Add(new ContentElementPermission(@event.IdMember, @event.MemberName, @event.MemberType, @event.RightType, ContentElementPermissionTypesEnum.Owner));
        }

        protected virtual void Handle(OwnerPermissionRemovedEvent @event)
        {
            _permissions.RemoveAll(p => p.IdMember == @event.IdMember && p.MemberType == @event.MemberType && p.PermissionType == ContentElementPermissionTypesEnum.Custom);
        }

        protected virtual void Handle(MemberPermissionSettedEvent @event)
        {
            _permissions.Add(new ContentElementPermission(@event.IdMember, @event.MemberName, @event.MemberType, @event.RightType, ContentElementPermissionTypesEnum.Custom));
        }

        protected virtual void Handle(MemberPermissionRemovedEvent @event)
        {
            _permissions.RemoveAll(p => p.IdMember == @event.IdMember && p.MemberType == @event.MemberType && p.PermissionType == ContentElementPermissionTypesEnum.Custom);
        }

        protected virtual void Handle(ContentElementReservedEvent @event)
        {
            Reserved = true;
            ReservedDate = @event.ReservedDate;
            ReserveIdUser = @event.ReserveIdUser;
        }

        protected virtual void Handle(ContentElementUnreservedEvent @event)
        {
            Reserved = false;
            ReservedDate = null;
            ReserveIdUser = null;
        }

        #endregion
    }
}
