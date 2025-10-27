// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Pi3.Core.SeedWork;
using System.ComponentModel.DataAnnotations;

namespace Pi3.App.RubricaComune.WebApi.Application.Commands.Corrispondenti.UpdateEmail
{
    public class DatiEmail : ValueObject
    {
        public bool? Preferita { get; init; } = null;

        public string? Note { get; init; } = null;
    }

    public class UpdateEmailRequest : ValueObject, IRequest
    {
        public string Id { get; init; } = null!;

        public string Email { get; init; } = null!;

        public DatiEmail DatiEmail { get; init; } = null!;
    }
}
