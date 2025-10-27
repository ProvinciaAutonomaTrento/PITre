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
    public class UtenteDestinatarioAddedEvent : Event
    {
        public UtenteDestinatarioAddedEvent()
        { }

        public string Id { get; init; }

        public string IdUtente { get; init; }

        public string? UserId { get; init; }

        public string? Cognome { get; init; }

        public string? Nome { get; init; }

        public string IdRagioneTrasmissione { get; init; }

        public string? NomeRagioneTrasmissione { get; init; }

        public CessioneDirittiRagioneTrasmissione? CessioneDirittiRagioneTrasmissione { get; init; }

        public TextValue? NoteTrasmissioneSingola { get; init; }

        public int? GiorniScadenza { get; init; }

        public bool? NascondiVersioniPrecedenti { get; init; }
    }
}
