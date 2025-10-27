// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
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
