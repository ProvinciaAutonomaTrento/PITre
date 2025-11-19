// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.AggregateModels.ElementAggregate.Repositories;
using Pi3.Core.AggregateModels.ElementAggregate;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pi3.Core.Test.ExampleAggregate;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;
using Pi3.Core.Services.Principal;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.Entities;

namespace Pi3.Core.Test.OrderAggregate.Repositories
{
    public class InMemoryOrderRepository : IOrderRepository
    {
        #region Public Members

        public async Task<bool> Exists(string idAggregate)
        {
            return await Task.Run(() =>
            {
                return this._orders.Any(a => a.Id.Equals(idAggregate));
            });
        }

        public async Task<Order> Get(string idAggregate, params ILoadBehavior[] loadBehaviors)
        {
            return await Task.Run(() =>
            {
                return this._orders.FirstOrDefault(a => a.Id.Equals(idAggregate));
            });
        }

        public async Task Add(Order aggregate)
        {
            await Task.Run(() =>
            {
                if (!aggregate.IsValid())
                    throw new InvalidAggregatePi3Exception(aggregate.GetErrors());

                this._orders.Add(aggregate);
                aggregate.MarkChangesAsCommitted();
            });
        }

        public async Task Update(Order aggregate)
        {
            await Task.Run(() =>
            {
                if (!aggregate.IsValid())
                    throw new InvalidAggregatePi3Exception(aggregate.GetErrors());

                this._orders.Remove(aggregate);
                this._orders.Add(aggregate);
                aggregate.MarkChangesAsCommitted();
            });
        }

        public async Task Delete(Order aggregate)
        {
            await Task.Run(() =>
            {
                this._orders.Remove(aggregate);
                aggregate.MarkChangesAsCommitted();
            });
        }

        public async Task Load(Order aggregate, params ILoadBehavior[] loadBehaviors)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Private Members

        protected readonly List<Order> _orders = new List<Order>();

        #endregion
    }
}
