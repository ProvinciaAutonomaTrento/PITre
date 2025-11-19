// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.App.RubricaComune.Core.AggregateModels.CorrispondenteAggregate.ValueObjects;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.RubricaComune.Core.AggregateModels.CorrispondenteAggregate.Events
{
    public class CorrispondenteCreatoEvent : Event
    {
        public CorrispondenteCreatoEvent() { }

        public string Codice { get; init; } = null!;

        public string Denominazione { get; init; } = null!;

        public TipiEnum Tipo { get; init; }

        public DateTime? DataCreazione { get; init; } = null;

        public DateTime? DataUltimaModifica { get; init; } = null;
    }
}
