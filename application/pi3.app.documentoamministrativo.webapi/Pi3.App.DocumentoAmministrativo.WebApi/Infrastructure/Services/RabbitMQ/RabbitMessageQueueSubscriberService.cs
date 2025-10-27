// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Infrastructure.Services.RabbitMessageQueue
{
    public class RabbitMessageQueueSubscriberService : IMessageQueueSubscriberService
    {
        private readonly IModel _channel;
        private readonly IMediator _mediator;

        public RabbitMessageQueueSubscriberService(IModel channel, IMediator mediator)
        {
            _channel = channel;
            _mediator = mediator;
        }

        public async Task SubscribeFromQueue<M>() where M : class
        {
            await Task.Run(() =>
            {
                var consumer = new AsyncEventingBasicConsumer(this._channel);

                var queueName = typeof(M).Name;
                this._channel.QueueDeclare(queueName, true, false, false);

                consumer.Received += async (s, e) =>
                {
                    var jsonSpecified = Encoding.UTF8.GetString(e.Body.Span);
                    var message = JsonConvert.DeserializeObject<M>(jsonSpecified);

                    await this._mediator.Publish(message);

                    await Task.Yield();
                };

                this._channel.BasicConsume(queueName, true, consumer);
            });
        }
    }
}
