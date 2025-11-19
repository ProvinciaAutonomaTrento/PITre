// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.Principal;
using Pi3.Core.Services.Principal;
using System.Security.Claims;

namespace Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.RabbitMQ
{
    public abstract class MessageQueueBaseCommandHandler<M> : IRequestHandler<M> where M : MessageQueueBaseCommand
    {
        protected readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;

        public MessageQueueBaseCommandHandler(ILogger logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        //public async Task Handle(M message, CancellationToken cancellationToken)
        //{
        //    using (var scope = _serviceProvider.CreateScope())
        //    {
        //        var claimsPrincipalService = scope.ServiceProvider.GetRequiredService<IClaimsPrincipalService>();

        //        ((ClaimsPrincipalService)claimsPrincipalService).Current =
        //            new ClaimsPrincipal(
        //                message.Identities
        //                    .Select(i => new ClaimsIdentity(
        //                        claims: i.Claims.Select(c => new Claim(c.Type, c.Value)),
        //                        authenticationType: i.AuthenticationType)));

        //        await this.InternalHandle(scope.ServiceProvider, message);
        //    }
        //}
        public async Task Handle(M message, CancellationToken cancellationToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var claimsPrincipalService = scope.ServiceProvider.GetRequiredService<ClaimsPrincipalService>();

                claimsPrincipalService.Current = new ClaimsPrincipal(
                    message.Identities
                        .Select(i => new ClaimsIdentity(
                            claims: i.Claims.Select(c => new Claim(c.Type, c.Value)),
                            authenticationType: i.AuthenticationType)));

                await this.InternalHandle(scope.ServiceProvider, message);
            }
        }

        protected abstract Task InternalHandle(IServiceProvider serviceProvider, M message);
    }
}
