// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.Extensions.DependencyInjection;
using Pi3.Core.Services.File.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Services.Factory
{
    public class ServiceProviderFactoryService : IFactoryService
    {
        #region Public Members

        public ServiceProviderFactoryService(IServiceProvider serviceProvider)
        {
            this._serviceProvider = serviceProvider;
        }

        public async virtual Task<(bool Success, S? Service)> TryCreate<S>(Func<S, bool> predicate) where S : class, IService
        {
            predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));

            var service = this._serviceProvider.GetServices<S>()!.FirstOrDefault(predicate);

            if (service != null)
                return new(true, service);
            else
                return new(false, default(S?));
        }

        public async virtual Task<S> Create<S>(Func<S, bool> predicate) where S : class, IService
        {
            predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));

            var creation = await this.TryCreate<S>(predicate);

            if (creation.Success)
                return creation.Service!;
            else
                throw new ServiceNotFoundPi3Exception();
        }

        #endregion

        #region Private Members

        protected readonly IServiceProvider _serviceProvider;

        #endregion
    }
}
