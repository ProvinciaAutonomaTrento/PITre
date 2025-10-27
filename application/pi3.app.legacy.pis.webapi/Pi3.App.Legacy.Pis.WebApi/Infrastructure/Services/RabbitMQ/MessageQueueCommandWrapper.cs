// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.RabbitMQ
{
    public sealed class MessageQueueCommandWrapper : IRequest
    {
        public MessageQueueCommandWrapper(MessageQueueBaseCommand command)
        {
            command = command ?? throw new ArgumentNullException(nameof(command));
            Validator.ValidateObject(command, new ValidationContext(command), true);

            Command = command;
        }

        public MessageQueueBaseCommand Command { get; init; }
    }
}
