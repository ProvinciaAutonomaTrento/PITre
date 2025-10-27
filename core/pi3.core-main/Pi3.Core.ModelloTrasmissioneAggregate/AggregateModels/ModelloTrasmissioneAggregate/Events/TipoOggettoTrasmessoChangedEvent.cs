// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.AggregateModels.ModelloTrasmissioneAggregate.ValueObjects;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.ModelloTrasmissioneAggregate.Events
{
    public class TipoOggettoTrasmessoChangedEvent : Event
    {
        public TipoOggettoTrasmessoChangedEvent()
        { }

        public string Id { get; init; }

        public TipiOggettiTrasmessiEnum NewTipoOggettoTrasmesso { get; init; }
    }
}
