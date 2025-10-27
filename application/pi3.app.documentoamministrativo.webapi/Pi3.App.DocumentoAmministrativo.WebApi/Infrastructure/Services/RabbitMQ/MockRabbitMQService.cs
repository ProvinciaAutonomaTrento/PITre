// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2

using MediatR;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Infrastructure.Services.RabbitMQ
{
    public class MockRabbitMQService : IRabbitMQService
    {
        private readonly IMediator _mediator;

        public MockRabbitMQService(IMediator mediator)
        {
            _mediator = mediator;
        }
        public async Task Publish(MessageQueueCommandWrapper commandWrapper)
        {
            await _mediator.Send(commandWrapper.Command);
        }

        public async Task Subscribe<C>(int retryCountLimit = 60, int secondsDelay = 10) where C : MessageQueueBaseCommand
        {
        }
    }
}
