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
    public class DatiPubblicazione : ValueObject
    {
        public DatiPubblicazione() { }

        [Required(AllowEmptyStrings = false)]
        public string Codice { get; init; } = null!;

        [Required]
        public string Denominazione { get; init; } = null!;

        [Required]
        public TipiEnum Tipo { get; init; }

        public Indirizzo? Indirizzo { get; init; } = null;

        public string? CodiceFiscale { get; init; } = null;

        public string? PartitaIva { get; init; } = null;
        
        public string? UrlApiInteroperabilita { get; init; } = null!;

        public string? AOO { get; init; } = null;

        public string? Amministrazione { get; init; } = null;

        [Required()]
        [MinLength(1)]
        public IReadOnlyList<EmailDaAggiornare> Emails { get; init; } = null!;
    }
}
