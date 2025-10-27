// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.App.Indexer.Core.AggregateModels.RequestAggregate.Events;
using Pi3.App.Indexer.Core.AggregateModels.RequestAggregate.ValueObjects;
using Pi3.Core.AggregateModels.ElementAggregate;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Indexer.Core.AggregateModels.RequestAggregate
{
    public class Request : Element
    {
        protected Request() : base()
        { }

        public Request(string id, string idTenant, DateTime requestDate, string idElement, RequestElementTypesEnum elementType, DateTime elementCreationDate, RequestOperationTypesEnum operationType, int remainingAttempts, DateTime? lastProcessingDate = null, double? lastProcessingElapsed = null, string? lastError = null, string? warnings = null)
        {
            this.ApplyChange(new RequestCreatedEvent()
            {
                Id = id,
                IdTenant = idTenant,
                IdElement = idElement,
                ElementType = elementType,
                ElementCreationDate = elementCreationDate,
                OperationType = operationType,
                RequestDate = requestDate,
                RemainingAttempts = remainingAttempts,
                LastProcessingDate = lastProcessingDate,
                LastProcessingElapsed = lastProcessingElapsed,
                LastError = lastError,
                Warnings = warnings
            });
        }

        public void UpdateRequestStatus(bool handled, DateTime? processingDate = null, double? processingElapsed = null, string? error = null, string? warnings = null)
        {
            this.ApplyChange(new RequestStatusUpdatedEvent()
            {
                IdRequest = this.Id,
                Handled = handled,
                ProcessingDate = processingDate ?? DateTime.Now,
                ProcessingElapsed = processingElapsed ?? 0,
                Error = error, 
                Warnings = warnings
            });
        }

        public string IdTenant { get; protected set; }

        public DateTime RequestDate { get; protected set; }

        public string IdUser { get; protected set; }

        public string IdRole { get; protected set; }

        public string IdElement { get; protected set; }

        public RequestElementTypesEnum ElementType { get; protected set; }
        
        public DateTime ElementCreationDate { get; protected set; }

        public RequestOperationTypesEnum OperationType { get; protected set; }

        public bool? Handled
        {
            get
            {
                return this._requestStatus?.Handled;
            }
        }

        public int? RemainingAttempts
        {
            get
            {
                return this._requestStatus?.RemainingAttempts;
            }
        }

        public DateTime? LastProcessingDate
        {
            get
            {
                return this._requestStatus?.LastProcessingDate;
            }
        }

        public double? LastProcessingElapsed
        {
            get
            {
                return this._requestStatus?.LastProcessingElapsed;
            }
        }

        public string? LastError
        {
            get
            {
                return this._requestStatus?.LastError;
            }
        }

        public string? Warnings
        {
            get
            {
                return this._requestStatus?.Warnings;
            }
        }

        protected RequestStatus _requestStatus = null;

        public void Handle(RequestCreatedEvent @event)
        {
            this.Id = @event.Id;
            this.IdTenant = @event.IdTenant;
            this.IdElement = @event.IdElement;
            this.ElementType = @event.ElementType;
            this.ElementCreationDate = @event.ElementCreationDate;
            this.OperationType = @event.OperationType;
            this.RequestDate = @event.RequestDate;
            this._requestStatus = new RequestStatus(@event.RemainingAttempts, false, @event.LastProcessingDate, @event.LastProcessingElapsed, @event.LastError, @event.Warnings);
        }

        public void Handle(RequestStatusUpdatedEvent @event)
        {
            this._requestStatus.UpdateStatus(@event.Handled, @event.ProcessingDate, @event.ProcessingElapsed, @event.Error, @event.Warnings);
        }
    }
}