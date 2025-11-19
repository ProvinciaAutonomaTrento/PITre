// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Pi3.App.InteropPitre.WebApi.Application.Commands.InteroperabilityService.ElaborateNewInteroperabilityMessage;
using Pi3.App.InteropPitre.WebApi.Application.Services.RabbitMQ;
using System.Security.Claims;

namespace Pi3.App.InteropPitre.WebApi.Application.Commands.InteroperabilityService.InviaEmailTrasmissione
{
    public class InviaEmailTrasmissioneRequest : MessageQueueBaseCommand
    {
        public InviaEmailTrasmissioneRequest()
          : base()
        { }

        public InviaEmailTrasmissioneRequest(ClaimsPrincipal claimsPrincipal)
            : base(claimsPrincipal)
        { }

        public long? IdTrasmissioneUtente { get; init; } = null!;

        public string? TipoNotifica { get; init; } = null!;
    }
}
