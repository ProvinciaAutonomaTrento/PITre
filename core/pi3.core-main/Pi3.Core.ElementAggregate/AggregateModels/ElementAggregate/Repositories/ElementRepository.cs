// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Pi3.Core.Services.Principal;
using System.Security.Claims;
using System.Runtime.CompilerServices;
using Pi3.Core.AggregateModels.ElementAggregate.Factories;

namespace Pi3.Core.AggregateModels.ElementAggregate.Repositories
{
    public abstract class ElementRepository<A>
        : Repository<A, string>, IElementRepository<A> where A : Element
    {
        #region Public Members

        public ElementRepository(ILogger logger, IClaimsPrincipalService claimsPrincipal, IEventPublisher eventPublisher)
            : base(logger, claimsPrincipal, eventPublisher)
        {
        }

        public async override Task<bool> Exists(string idAggregate)
        {
            throw new MethodNotImplementedPi3Exception(nameof(Exists));
        }

        public virtual async Task<bool> Exists(string idTenant, string id)
        {
            _claimsPrincipal.Current.AssertPi3IdentityAuthenticated();

            return await HandleExists(idTenant, id);
        }

        public async override Task<A> Get(string idAggregate, ILoadBehavior[]? loadBehaviors = null)
        {
            throw new MethodNotImplementedPi3Exception(nameof(Get));
        }

        public virtual async Task<A> Get(string idTenant, string id, ILoadBehavior[]? loadBehaviors = null)
        {
            _claimsPrincipal.Current.AssertPi3IdentityAuthenticated();

            var aggregate = ElementFactory.Create<A>();

            aggregate = await HandleGet(aggregate, idTenant, id, loadBehaviors);

            return aggregate;

        }

        #endregion

        #region Private Members

        protected abstract Task<bool> HandleExists(string idTenant, string id);

        protected override Task<bool> HandleExists(string id)
        {
            throw new MethodNotImplementedPi3Exception(nameof(HandleExists));
        }

        protected abstract Task<A> HandleGet(A newAggregate, string idTenant, string id, ILoadBehavior[]? loadBehaviors = null);

        protected override Task<A> HandleGet(A newAggregate, string id, params ILoadBehavior[] loadBehaviors)
        {
            throw new MethodNotImplementedPi3Exception(nameof(HandleGet));
        }

        #endregion
    }
}
