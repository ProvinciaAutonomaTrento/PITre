// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
namespace Pi3.App.DocumentoAmministrativo.WebApi.Infrastructure.Services.RabbitMessageQueue
{
    public interface IMessageQueuePublisherService
    {
        Task PublishToQueue<M>(M message) where M : class;
    }
}
