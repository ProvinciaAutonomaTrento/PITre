// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pi3.Core.Services.Principal;
using System.Reflection;
using System.Xml.Linq;

namespace Pi3.Core.SeedWork
{
    public abstract class Repository<A, K> :
        IRepository<A, K> where A : AggregateRoot<K>
    {
        #region Public Members

        public Repository(ILogger logger, IClaimsPrincipalService claimsPrincipal, IEventPublisher eventPublisher)
        {
            _logger = logger;
            _claimsPrincipal = claimsPrincipal;
            _eventPublisher = eventPublisher;
        }
               
        public virtual async Task<bool> Exists(K id)
        {
            this._claimsPrincipal.Current.AssertPi3IdentityAuthenticated();

            return await this.HandleExists(id);
        }

        public virtual async Task<A> Get(K id, params ILoadBehavior[] loadBehaviors)
        {
            this._claimsPrincipal.Current.AssertPi3IdentityAuthenticated();

            var aggregate = AggregateRootFactory.Create<A, K>();

            aggregate = await this.HandleGet(aggregate, id, loadBehaviors);

            return aggregate;
        }

        public virtual async Task Add(A aggregate)
        {
            this._claimsPrincipal.Current.AssertPi3IdentityAuthenticated();

            if (!aggregate.IsValid())
                throw new InvalidAggregatePi3Exception(aggregate.GetErrors());

            await this.HandleAdd(aggregate);

            await this.PublishChanges(aggregate);
        }

        public virtual async Task Delete(A aggregate)
        {
            _claimsPrincipal.Current.AssertPi3IdentityAuthenticated();

            if (!aggregate.IsValid())
                throw new InvalidAggregatePi3Exception(aggregate.GetErrors());

            await this.HandleDelete(aggregate);

            await this.PublishChanges(aggregate);
        }

        public virtual async Task Update(A aggregate)
        {
            _claimsPrincipal.Current.AssertPi3IdentityAuthenticated();

            if (!aggregate.IsValid())
                throw new InvalidAggregatePi3Exception(aggregate.GetErrors());

            await this.HandleUpdate(aggregate);

            await this.PublishChanges(aggregate);
        }

        public virtual async Task Load(A aggregate, params ILoadBehavior[] loadBehaviors)
        {
            _claimsPrincipal.Current.AssertPi3IdentityAuthenticated();

            foreach (var loadBehavior in loadBehaviors)
            {
                var handleLoadMethod = this.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                    .FirstOrDefault(m => m.Name == "HandleLoadBehavior"
                        && m.GetParameters().Count(p => p.ParameterType == loadBehavior.GetType()) > 0);

                if (handleLoadMethod == null)
                    throw new MethodNotImplementedPi3Exception("HandleLoadBehavior");

                handleLoadMethod.Invoke(this, new object[] { aggregate, loadBehavior });
            }
        }

        #endregion

        #region Private Members

        protected readonly ILogger _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipal;
        protected readonly IEventPublisher _eventPublisher;

        protected abstract Task<bool> HandleExists(K id);

        protected abstract Task<A> HandleGet(A newAggregate, K id, params ILoadBehavior[] loadBehaviors);

        protected abstract Task HandleAdd(A aggregate);

        protected abstract Task HandleDelete(A aggregate);

        protected abstract Task HandleUpdate(A aggregate);

        protected virtual void LoadAggregateFromHistory(A aggregate, params IEvent[] history)
        {
            aggregate.LoadFromHistory(history);
        }

        protected virtual async Task PublishChanges(A aggregate)
        {
            foreach (var change in aggregate.GetUncommittedChanges())
                await this._eventPublisher.Publish(change);

            aggregate.MarkChangesAsCommitted();
        }

        #endregion
    }
}