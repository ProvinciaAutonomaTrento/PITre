// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels
{
    public class SyncEventPublisher : IEventPublisher
    {   
        #region Public Members

        public SyncEventPublisher(IServiceProvider serviceProvider)
        {
            this._serviceProvider = serviceProvider;
        }

        public virtual async Task Publish<E>(E @event) where E : IEvent
        {
            var handlers = (await this.GetEventHandlers<E>())?.ToList();

            foreach (var eventHandler in handlers)
            {
                await eventHandler.Handle(@event);
            }
        }

        #endregion

        #region Private Members

        protected readonly IServiceProvider _serviceProvider;

        protected virtual async Task<IEnumerable<IEventHandler<E>>> GetEventHandlers<E>() where E : IEvent
        {
            return _serviceProvider.GetServices<IEventHandler<E>>();
        }

        #endregion
    }
}
