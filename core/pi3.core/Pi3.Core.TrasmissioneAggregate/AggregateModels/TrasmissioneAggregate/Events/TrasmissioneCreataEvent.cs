// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.AggregateModels.ElementAggregate.Events;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.ValueObjects;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.TrasmissioneAggregate.Events
{
    public class TrasmissioneCreataEvent : ElementCreatedEvent
    {
        public TrasmissioneCreataEvent()
        { }

        public Autore? Autore { get; init; }

        public string IdOggettoTrasmesso { get; init; }

        public TipiOggettiTrasmessiEnum TipoOggettoTrasmesso { get; init; }

        public TextValue? NoteGenerali { get; init; }
    }
}
