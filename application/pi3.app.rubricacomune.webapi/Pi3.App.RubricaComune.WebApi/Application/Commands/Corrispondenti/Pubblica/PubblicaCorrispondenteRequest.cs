// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Pi3.Core.SeedWork;
using System.ComponentModel.DataAnnotations;

namespace Pi3.App.RubricaComune.WebApi.Application.Commands.Corrispondenti.Pubblica
{
    public class Email : ValueObject
    {
        [Required(AllowEmptyStrings = false)]
        public string? Indirizzo { get; init; } = null;

        public bool? Preferita { get; init; } = null;

        public string? Note { get; init; } = null;
    }

    public class PubblicaCorrispondenteRequest : IRequest<PubblicaCorrispondenteResponse>
    {
        [Required(AllowEmptyStrings = false)]
        public string? Codice { get; init; } = null;

        [Required]
        public DatiCorrispondente DatiCorrispondente { get; set; } = null!;

        public IReadOnlyList<Email>? Emails { get; init; } = null;
    }
}
