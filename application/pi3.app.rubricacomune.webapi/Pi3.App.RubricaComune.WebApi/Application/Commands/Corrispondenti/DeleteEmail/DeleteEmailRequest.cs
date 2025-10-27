// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Pi3.Core.SeedWork;

namespace Pi3.App.RubricaComune.WebApi.Application.Commands.Corrispondenti.DeleteEmail
{
    public class DeleteEmailRequest : ValueObject, IRequest
    {
        public string Id { get; init; } = null!;

        public string Email { get; init; } = null!;
    }
}
