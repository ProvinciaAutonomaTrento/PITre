// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.DocumentAggregate.Events
{
    public class DocumentVersionNameChangedEvent : Event
    {
        public DocumentVersionNameChangedEvent() : base()
        {
        }

        public string IdVersion { get; init; } = null;

        public TextValue? NewName { get; init; } = null;
    }
}
