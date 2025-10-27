// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Pi3.Core.SeedWork;
using System.ComponentModel.DataAnnotations;

namespace Pi3.App.RubricaComune.WebApi.Application.Commands.Corrispondenti.PubblicaAggiornamento
{
    public class Email : ValueObject
    {
        [Required(AllowEmptyStrings = false)]
        public string Indirizzo { get; init; } = null!;

        public bool? Preferita { get; init; } = null;

        public string? Note { get; init; } = null;
    }

    public class DatiAggiornamento : ValueObject
    {
        [Required]
        public DatiCorrispondente DatiCorrispondente { get; set; } = null!;

        public IReadOnlyList<Email>? Emails { get; init; } = null;
    }

    public class PubblicaAggiornamentoRequest : IRequest
    {
        [Required(AllowEmptyStrings = false)]
        public string Id { get; init; } = null!;

        [Required]
        public DatiAggiornamento? DatiAggiornamento { get; init; }
    }
}
