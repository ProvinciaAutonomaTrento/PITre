// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Infrastructure.Services.RabbitMessageQueue
{
    public class RabbitMessageQueuePublisherService : IMessageQueuePublisherService
    {
        private readonly IModel _channel;

        public RabbitMessageQueuePublisherService(IModel channel)
        {
            _channel = channel;
        }

        public async Task PublishToQueue<M>(M message) where M : class
        {
            await Task.Run(() =>
            {
                var properties = this._channel.CreateBasicProperties();
                properties.Persistent = true;
                var output = JsonConvert.SerializeObject(message);
                this._channel.BasicPublish(string.Empty, message.GetType().Name, null, Encoding.UTF8.GetBytes(output));
            });
        }
    }
}
