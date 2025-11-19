// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.AggregateModels.ModelloTrasmissioneAggregate.ValueObjects;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.ModelloTrasmissioneAggregate.Events
{
    public class GruppoDestinatarioAddedEvent : Event
    {
        public GruppoDestinatarioAddedEvent()
        { }

        public string Id { get; init; }

        public string IdGruppo { get; init; }

        public string? CodiceGruppo { get; init; }

        public TextValue? DescrizioneGruppo { get; init; }

        public string IdRagioneTrasmissione { get; init; }

        public string? NomeRagioneTrasmissione { get; init; }

        public CessioneDirittiRagioneTrasmissione? CessioneDirittiRagioneTrasmissione { get; init; }

        public TipiTrasmissioneSingolaEnum TipoTrasmissioneSingola { get; init; }

        public List<DatiUtenteNotificato> UtentiNotificati { get; init; }

        public TextValue? NoteTrasmissioneSingola { get; init; }

        public int? GiorniScadenza { get; init; }

        public bool? NascondiVersioniPrecedenti { get; init; }
    }
}
