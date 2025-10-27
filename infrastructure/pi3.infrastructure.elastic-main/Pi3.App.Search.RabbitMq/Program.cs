// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Doc.Application.Search.DocumentoAmministrativo.Commands;
using MediatR;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Pi3.App.Search.RabbitMq;

class Program
{
    static async Task Main(string[] args)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(queue: "indexDocumentoAmministrativo",
                             durable: false,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine(" [x] Received {0}", message);

            var request = JsonSerializer.Deserialize<Request>(message);
        };

        await channel.BasicConsumeAsync(queue: "hello",
                             autoAck: true,
                             consumer: consumer);

        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
    }

    protected virtual async Task<Pi3.Search.DocumentoAmministrativo.Models.RequestStatus> IndexDocument(string idTenant, string idDocument, RequestOperationTypesEnum operationType)
    {
        Pi3.Search.DocumentoAmministrativo.Models.RequestStatus requestStatus = null;
        var mediator = this._services.GetService<IMediator>();

        switch (operationType)
        {
            case RequestOperationTypesEnum.AddOrUpdate:
                requestStatus = (await mediator.Send(new AddOrUpdateDocumentoAmministrativoCommand(idTenant, idDocument))).RequestStatus;

                break;

            case RequestOperationTypesEnum.Delete:
                requestStatus = (await mediator.Send(new DeleteDocumentoAmministrativoCommand(idTenant, idDocument))).RequestStatus;

                break;
        }

        return requestStatus;
    }
}