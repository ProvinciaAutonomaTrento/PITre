// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Services.RabbitMQ
{
    public interface IRabbitMQService : IService
    {
        Task Publish(MessageQueueCommandWrapper commandWrapper);

        Task Subscribe<C>(int retryCountLimit = 60, int secondsDelay = 10) where C : MessageQueueBaseCommand;
    }
}
