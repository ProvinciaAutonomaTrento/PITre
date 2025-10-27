// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Pi3.App.Legacy.WebApi.Application.Services.RabbitMQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Requests
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
