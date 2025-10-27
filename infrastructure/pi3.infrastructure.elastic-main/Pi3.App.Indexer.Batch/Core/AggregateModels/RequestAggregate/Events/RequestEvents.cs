// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.App.Indexer.Core.AggregateModels.RequestAggregate.ValueObjects;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Indexer.Core.AggregateModels.RequestAggregate.Events
{
    public class RequestCreatedEvent : Event
    {
        public string Id { get; init; }
        public string IdTenant { get; init; }
        public DateTime RequestDate { get; init; }
        public string IdElement { get; init; }
        public RequestElementTypesEnum ElementType { get; init; }
        public DateTime ElementCreationDate { get; init; }
        public RequestOperationTypesEnum OperationType { get; init; }
        public int RemainingAttempts { get; init; }
        public DateTime? LastProcessingDate { get; init; }
        public double? LastProcessingElapsed { get; init; }
        public string? LastError { get; init; }
        public string? Warnings { get; init; }
    }

    public class RequestStatusUpdatedEvent : Event
    {
        public string IdRequest { get; init; }
        public bool Handled { get; init; }
        public DateTime ProcessingDate { get; init; }
        public double ProcessingElapsed { get; init; }
        public string? Error { get; init; }
        public string? Warnings { get; init; }
    }
}
