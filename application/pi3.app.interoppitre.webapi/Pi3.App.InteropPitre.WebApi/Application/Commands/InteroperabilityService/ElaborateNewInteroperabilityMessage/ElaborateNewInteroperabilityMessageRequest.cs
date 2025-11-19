// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Pi3.App.InteropPitre.WebApi.Application.Commands.InteroperabilityService.Domain;
using System.ComponentModel.DataAnnotations;

namespace Pi3.App.InteropPitre.WebApi.Application.Commands.InteroperabilityService.ElaborateNewInteroperabilityMessage
{
    public class ElaborateNewInteroperabilityMessageRequest : IRequest<ElaborateNewInteroperabilityMessageResponse>
    {
        [Required]
        public InteroperabilityMessage InteroperabilityMessage { get; set; }
    }
}
