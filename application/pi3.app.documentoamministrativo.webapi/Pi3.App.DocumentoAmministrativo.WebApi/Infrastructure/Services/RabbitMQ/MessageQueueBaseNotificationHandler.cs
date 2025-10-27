// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Pi3.App.DocumentoAmministrativo.WebApi.Infrastructure.Services.Principal;
using Pi3.Core.Services.Principal;
using System.Security.Claims;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Infrastructure.Services.RabbitMQ
{
    public abstract class MessageQueueBaseNotificationHandler<N> : INotificationHandler<N> where N : MessageQueueBaseNotification
    {
        protected readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;

        public MessageQueueBaseNotificationHandler(ILogger logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public async Task Handle(N notification, CancellationToken cancellationToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var claimsPrincipalService = scope.ServiceProvider.GetRequiredService<IClaimsPrincipalService>();
                ((ClaimsPrincipalService) claimsPrincipalService).Current = new ClaimsPrincipal(
                    new ClaimsIdentity(
                        claims: notification.Claims.Select(c => new Claim(c.Type, c.Value)),
                        authenticationType: "Pi3Authentication"));

                await this.InternalHandle(scope.ServiceProvider, notification);
            }
        }

        protected abstract Task InternalHandle(IServiceProvider serviceProvider, N notification);
    }
}
