// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.RabbitMQ;
using System.Security.Claims;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Transmissions.InviaEmailTrasmissione
{
    public class InviaEmailTrasmissioneCommand : MessageQueueBaseCommand
    {

        public InviaEmailTrasmissioneCommand()
          : base()
        { }

        public InviaEmailTrasmissioneCommand(ClaimsPrincipal claimsPrincipal)
            : base(claimsPrincipal)
        { }

        public long? IdTrasmissioneUtente { get; init; } = null!;

        public string? TipoNotifica { get; init; } = null!;
    }
}
