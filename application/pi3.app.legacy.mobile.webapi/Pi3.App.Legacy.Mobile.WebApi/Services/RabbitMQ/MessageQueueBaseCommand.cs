// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using System.Security.Claims;

namespace Pi3.App.Legacy.Mobile.WebApi.Services.RabbitMQ
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
