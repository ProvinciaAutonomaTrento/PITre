// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.RubricaComune.Core.AggregateModels.CorrispondenteAggregate.ValueObjects
{
    public class DatiPubblicazioneAggiornamento : ValueObject
    {
        public DatiPubblicazioneAggiornamento() { }

        [Required]
        public string Denominazione { get; init; } = null!;

        public Indirizzo? Indirizzo { get; init; } = null;

        public string? CodiceFiscale { get; init; } = null;

        public string? PartitaIva { get; init; } = null;

        [Required(AllowEmptyStrings = false)]
        public string UrlApiInteroperabilita { get; init; } = null!;

        public string? AOO { get; init; } = null;

        public string? Amministrazione { get; init; } = null;

        [Required()]
        [MinLength(1)]
        public IReadOnlyList<EmailDaAggiornare> Emails { get; init; } = null!;
    }
}
