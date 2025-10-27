// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Pi3.Core.Services.Principal;
using System.Linq;
using System.Security.Claims;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Infrastructure.Services.RabbitMQ
{
    public abstract class MessageQueueBaseCommand : IRequest
    {
        public MessageQueueBaseCommand()
        {
            this.Identities = new List<MessageQueueCommandIdentity>();
        }

        public MessageQueueBaseCommand(ClaimsPrincipal claimsPrincipal) : this()
        {
            this.Identities.AddRange(
                claimsPrincipal.Identities
                    .Select(i => new MessageQueueCommandIdentity(
                        i.AuthenticationType ?? string.Empty,
                        i.Claims
                            .Select(c => 
                            new MessageQueueCommandClaim(c.Type, c.Value))
                            .ToList())));
        }

        public List<MessageQueueCommandIdentity> Identities
        {
            get;
            set;
        }
    }
}
