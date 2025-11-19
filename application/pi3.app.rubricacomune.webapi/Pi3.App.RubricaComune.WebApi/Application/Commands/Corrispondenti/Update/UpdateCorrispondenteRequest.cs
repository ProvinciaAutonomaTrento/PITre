// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Pi3.App.RubricaComune.WebApi.Application.Commands.Corrispondenti.Create;
using Pi3.Core.SeedWork;
using System.ComponentModel.DataAnnotations;

namespace Pi3.App.RubricaComune.WebApi.Application.Commands.Corrispondenti.Update
{
    public class UpdateCorrispondenteRequest : ValueObject, IRequest
    {
        [Required(AllowEmptyStrings = false)]
        public string Id { get; init; } = null!;

        [Required]
        public DatiCorrispondente DatiCorrispondente { get; init; } = null!;
    }
}
