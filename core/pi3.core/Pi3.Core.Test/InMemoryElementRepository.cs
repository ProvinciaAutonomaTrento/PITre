// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.Extensions.Logging;
using Pi3.Core.AggregateModels.DocumentAggregate.Events;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.AggregateModels.ElementAggregate;
using Pi3.Core.AggregateModels.ElementAggregate.Repositories;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Pi3.Core.Test
{
    public class InMemoryElementRepository<A> : IElementRepository<A> where A : Element
    {
        protected readonly List<A> _elements;

        public InMemoryElementRepository()
        {
            this._elements = new List<A>();
        }

        public async Task Add(A element)
        {
            await Task.Run(() =>
            {
                if (!element.IsValid())
                    throw new InvalidAggregatePi3Exception(element.GetErrors());

                if (element.Id == null)
                    element.AssignId(Guid.NewGuid().ToString());

                this._elements.Add(element);
                element.MarkChangesAsCommitted();
            });
        }

        public async Task Delete(A element)
        {
            await Task.Run(() =>
            {
                this._elements.Remove(element);
                element.MarkChangesAsCommitted();
            });
        }
        public Task<bool> Exists(string idElement)
        {
            throw new MethodNotImplementedPi3Exception(nameof(Exists));
        }

        public async Task<bool> Exists(string idTenant, string idElement)
        {
            return await Task.Run(() =>
            {
                return this._elements.Any(a => a.Id.Equals(idElement));
            });
        }

        public async Task<A> Get(string idElement, params ILoadBehavior[] loadBehaviors)
        {
            throw new MethodNotImplementedPi3Exception(nameof(Get));
        }

        public async Task<A> Get(string idTenant, string idElement, params ILoadBehavior[] loadBehaviors)
        {
            return await Task.Run(() =>
            {
                return this._elements.FirstOrDefault(a => a.Id.Equals(idElement));
            });
        }

        public async Task Update(A element)
        {
            await Task.Run(() =>
            {
                if (!element.IsValid())
                    throw new InvalidAggregatePi3Exception(element.GetErrors());

                this._elements.Remove(element);
                this._elements.Add(element);
                element.MarkChangesAsCommitted();
            });
        }

        public async Task Load(A element, params ILoadBehavior[] loadBehaviors)
        {
        }
    }
}
