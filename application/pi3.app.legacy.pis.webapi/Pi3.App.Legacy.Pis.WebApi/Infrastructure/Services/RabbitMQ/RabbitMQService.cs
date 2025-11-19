// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Elastic.Apm;
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

namespace Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.RabbitMQ
{
    public class RabbitMQService : IRabbitMQService
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

                this._channel.BasicPublish(
                    exchange: string.Empty,
                    routingKey: routingKey,
                    basicProperties: properties,
                    body: Encoding.UTF8.GetBytes(output));
            });
        }

        public async Task Subscribe<C>(int retryCountLimit = 60, int secondsDelay = 10) where C : MessageQueueBaseCommand
        {
            await Task.Run(() =>
            {
                var consumer = new AsyncEventingBasicConsumer(this._channel);

                var queueName = $"{typeof(C).Assembly.GetName().Name}.{typeof(C).Name}";
                var retryQueueName = $"{typeof(C).Assembly.GetName().Name}.{typeof(C).Name}.Retry";
                var deadLetterQueueName = $"{typeof(C).Assembly.GetName().Name}.{typeof(C).Name}.Dlq";

                // Dichiarazione della coda principale
                this._channel.QueueDeclare(
                    queue: queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false);

                var retryQueueArgs = new Dictionary<string, object>
                {
                    { "x-message-ttl", (int) TimeSpan.FromSeconds(secondsDelay).TotalMilliseconds }, // Time To Live
                    { "x-dead-letter-exchange", "" },              // Torna alla main queue
                    { "x-dead-letter-routing-key", queueName }  // Routing key per tornare alla coda principale
                };

                // Dichiarazione della coda di retry
                this._channel.QueueDeclare(
                    queue: retryQueueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: retryQueueArgs);

                // Dichiarazione della coda di dead letter
                this._channel.QueueDeclare(
                    queue: deadLetterQueueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false);

                consumer.Received += async (s, e) =>
                {
                    // Avvia una nuova transazione APM per tracciare il metodo del consumer
                    var transaction = Agent.Tracer.StartTransaction(e.RoutingKey, "messaging");

                    try
                    {
                        var jsonSpecified = Encoding.UTF8.GetString(e.Body.Span);

                        var command = JsonConvert.DeserializeObject<C>(jsonSpecified);

                        await this._mediator.Send(command);

                        // Invia l'ACK dopo l'elaborazione
                        this._channel.BasicAck(deliveryTag: e.DeliveryTag, multiple: false);
                    }
                    catch (Exception ex)
                    {
                        var pi3Ex = new RabbitMQReceivedPi3Exception(ex);

                        this._logger.LogError(exception: pi3Ex, message: pi3Ex.Message);

                        // Leggi o inizializza il contatore dei retry
                        var headers = e.BasicProperties.Headers ?? new Dictionary<string, object>();
                        int retryCount = headers.ContainsKey("x-retry-count")
                            ? Convert.ToInt32(headers["x-retry-count"])
                            : 0;

                        retryCount++;

                        if (retryCount >= retryCountLimit) // Limite massimo di tentativi
                        {
                            this._logger.LogInformation($"Messaggio scartato dopo {retryCount} tentativi: {Encoding.UTF8.GetString(e.Body.ToArray())}");

                            // Invia il messaggio alla DLQ
                            var dlqProperties = this._channel.CreateBasicProperties();
                            dlqProperties.Persistent = true;
                            this._channel.BasicPublish(
                                exchange: string.Empty,
                                routingKey: deadLetterQueueName,
                                basicProperties: dlqProperties,
                                body: e.Body.ToArray());

                            // ACK per evitare ulteriori tentativi
                            this._channel.BasicAck(deliveryTag: e.DeliveryTag, multiple: false);
                        }
                        else
                        {
                            this._logger.LogInformation($"Retry #{retryCount} per il messaggio: {Encoding.UTF8.GetString(e.Body.ToArray())}");

                            // Incrementa il contatore dei retry e reimmetti nella coda
                            var retryProperties = this._channel.CreateBasicProperties();
                            retryProperties.Headers = headers;
                            retryProperties.Headers["x-retry-count"] = retryCount;

                            // Rifiuta e reimmetti nella stessa coda
                            this._channel.BasicPublish(
                                exchange: string.Empty,
                                routingKey: $"{e.RoutingKey}.Retry", // Invia alla coda con delay
                                basicProperties: retryProperties,
                                body: e.Body.ToArray());

                            // Rifiuta il messaggio originale senza requeue
                            this._channel.BasicAck(deliveryTag: e.DeliveryTag, multiple: false);
                        }
                    }
                    finally
                    {
                        transaction.End(); // Termina la transazione APM
                    }

                    await Task.Yield();
                };

                this._channel.BasicConsume(queueName, false, consumer);
            });
        }
    }
}
