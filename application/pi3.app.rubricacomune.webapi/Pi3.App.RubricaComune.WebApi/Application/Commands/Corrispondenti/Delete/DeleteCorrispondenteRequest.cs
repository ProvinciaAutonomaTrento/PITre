// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Pi3.App.RubricaComune.WebApi.Application.Commands.Corrispondenti.Delete
{
    public class DeleteCorrispondenteRequest : IRequest
    {
        [Required(AllowEmptyStrings = false)]
        public string Id { get; init; } = null!;
    }
}
