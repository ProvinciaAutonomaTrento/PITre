// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
namespace Pi3.App.DocumentoAmministrativo.WebApi.Infrastructure.Services.RabbitMessageQueue
{
    public interface IMessageQueueSubscriberService
    {
        Task SubscribeFromQueue<M>() where M : class;
    }
}
