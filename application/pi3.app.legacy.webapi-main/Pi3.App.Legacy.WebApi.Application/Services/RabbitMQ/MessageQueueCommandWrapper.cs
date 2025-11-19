// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Pi3.App.Legacy.WebApi.Application.Services.RabbitMQ
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
