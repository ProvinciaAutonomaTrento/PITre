// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Pi3.Core.Services.Principal;
using RabbitMQ.Client;
using System.Text;

namespace Pi3.App.Legacy.Mobile.WebApi.Services.RabbitMQ
{
    public sealed class RabbitMQCommandHandler : IRequestHandler<MessageQueueCommandWrapper> 
    {
        private readonly ILogger<RabbitMQCommandHandler> _logger;
        private readonly RabbitMQService _rabbitMQService;

        public RabbitMQCommandHandler(
            ILogger<RabbitMQCommandHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService,
            RabbitMQService rabbitMQService)
        {
            _logger = logger;
            _rabbitMQService = rabbitMQService;
        }

        public async Task Handle(MessageQueueCommandWrapper command, CancellationToken cancellationToken)
        {
            await this._rabbitMQService.Publish(command);
        }
    }
}
