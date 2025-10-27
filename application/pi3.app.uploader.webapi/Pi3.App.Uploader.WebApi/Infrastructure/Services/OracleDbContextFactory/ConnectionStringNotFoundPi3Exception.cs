// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;

namespace Pi3.App.Uploader.WebApi.Infrastructure.Services.OracleDbContextFactory
{

    public class ConnectionStringNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public ConnectionStringNotFoundPi3Exception(string tenant)
            : base(Resources.ErrorDescriptions.ConnectionStringNotFound, null, Resources.ErrorDescriptions.ResourceManager, tenant)
        {
            this.Tenant = tenant;
        }

        public string Tenant { get; init; }

        #endregion
    }
}
