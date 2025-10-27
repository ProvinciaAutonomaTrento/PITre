// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.ListaDistribuzioneAggregate.Events
{
    public class AutorePersonaAssignedEvent : Event
    {
        public AutorePersonaAssignedEvent()
        { }

        public string Id { get; init; }

        public string IdUtente { get; init; }

        public string? UserId { get; init; }

        public string? Cognome { get; init; }

        public string? Nome { get; init; }
    }

}
