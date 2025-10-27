// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.AggregateModels.TrasmissioneAggregate.ValueObjects;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.TrasmissioneAggregate.Events
{

    public class TrasmissioneSingolaUtenteAddedEvent : Event
    {
        public TrasmissioneSingolaUtenteAddedEvent()
        { }

        public string Id { get; init; }

        public string IdTrasmissioneSingola { get; init; }

        public string IdRagioneTrasmissione { get; init; }

        public string? NomeRagioneTrasmissione { get; init; }

        public bool? RagioneConWorkflow { get; init; }

        public CessioneDirittiRagione? CessioneDirittiRagione { get; set; }

        public string IdUtenteDestinatario { get; init; }

        public string? UserIdDestinatario { get; init; }

        public string? CognomeDestinatario { get; init; }

        public string? NomeDestinatario { get; init; }

        public TextValue? Note { get; init; }

        public DateTime? DataScadenza { get; init; }

        public bool? NascondiVersioniPrecedenti { get; init; }
    }

}
