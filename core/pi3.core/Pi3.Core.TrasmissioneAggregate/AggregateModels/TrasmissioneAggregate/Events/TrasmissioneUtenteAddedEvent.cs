// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.TrasmissioneAggregate.Events
{
    public class TrasmissioneUtenteAddedEvent : Event
    {
        public TrasmissioneUtenteAddedEvent()
        { }

        public string Id { get; init; }

        public string IdTrasmissioneSingola { get; init; }

        public string IdTrasmissioneUtente { get; init; }

        public string IdUtente { get; init; }

        public string? UserId { get; init; }

        public string? Cognome { get; init; }

        public string? Nome { get; init; }

        public DateTime? DataRimozioneCentroNotifiche { get; init; }
    }
}
