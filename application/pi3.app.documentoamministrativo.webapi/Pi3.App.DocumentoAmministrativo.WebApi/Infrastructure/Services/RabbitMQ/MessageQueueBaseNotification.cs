// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Pi3.Core.Services.Principal;
using System.Security.Claims;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Infrastructure.Services.RabbitMQ
{
    public abstract class MessageQueueBaseNotification : INotification
    {
        public MessageQueueBaseNotification()
        { }

        public MessageQueueBaseNotification(ClaimsPrincipal claimsPrincipal)
        {
            this.Claims = new List<MessageQueueClaim>(
                claimsPrincipal.GetPi3Identity().Claims.Select(
                    c => new MessageQueueClaim(c.Type, c.Value)));
        }

        public List<MessageQueueClaim> Claims { get; init; } = null!;
    }
}
