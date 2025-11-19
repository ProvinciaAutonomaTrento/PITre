// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
namespace Pi3.App.Legacy.WebApi.Application.Services.RabbitMQ
{
    public class MessageQueueCommandClaim
    {
        public MessageQueueCommandClaim(string type, string value)
        {
            Type = type;
            Value = value;
        }

        public string Type { get; init; }
        public string Value { get; init; }
    }
}
