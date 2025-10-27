// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
namespace Pi3.App.DocumentoAmministrativo.WebApi.Infrastructure.Services.RabbitMQ
{
    public class MessageQueueCommandIdentity
    {
        public MessageQueueCommandIdentity(string authenticationType, List<MessageQueueCommandClaim> claims) 
        {
            this.AuthenticationType = authenticationType;
            this.Claims = claims;
        }

        public string? AuthenticationType { get; init; } = null;

        public List<MessageQueueCommandClaim> Claims { get; init; }
    }
}
