// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Pi3.Core.SeedWork;
using Pi3.Core.Services;
using Pi3.Core.Services.Principal;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Security.Claims;
using System.Text;

namespace Pi3.App.Legacy.Mobile.WebApi.Services.RabbitMQ
{
    public class RabbitMQService : IService
    {
        private readonly ILogger<RabbitMQService> _logger;
        private readonly IModel _channel;
        private readonly IMediator _mediator;

        public RabbitMQService(ILogger<RabbitMQService> logger, IModel channel, IMediator mediator)
        {
            _logger = logger;
            _channel = channel;
            _mediator = mediator;
        }

        public async Task Publish(MessageQueueCommandWrapper commandWrapper)
        {
            await Task.Run(() =>
            {
                var properties = this._channel.CreateBasicProperties();
                properties.Persistent = true;

                var output = JsonConvert.SerializeObject(commandWrapper.Command);

                var routingKey = $"{commandWrapper.Command.GetType().Assembly.GetName().Name}.{commandWrapper.Command.GetType().Name}";

                this._channel.BasicPublish(string.Empty, routingKey, null, Encoding.UTF8.GetBytes(output));
            });
        }

        public async Task Subscribe<C>() where C : MessageQueueBaseCommand
        {
            await Task.Run(() =>
            {
                var consumer = new AsyncEventingBasicConsumer(this._channel);

                var queueName = $"{typeof(C).Assembly.GetName().Name}.{typeof(C).Name}";
                this._channel.QueueDeclare(queueName, true, false, false);

                consumer.Received += async (s, e) =>
                {
                    try
                    {
                        var jsonSpecified = Encoding.UTF8.GetString(e.Body.Span);

                        var command = JsonConvert.DeserializeObject<C>(jsonSpecified);

                        await this._mediator.Send(command);
                    }
                    catch (Exception ex)
                    {
                        var pi3Ex = new RabbitMQReceivedPi3Exception(ex);

                        this._logger.LogError(exception: pi3Ex, message: pi3Ex.Message);
                    }

                    await Task.Yield();
                };

                this._channel.BasicConsume(queueName, true, consumer);
            });
        }
    }
}
