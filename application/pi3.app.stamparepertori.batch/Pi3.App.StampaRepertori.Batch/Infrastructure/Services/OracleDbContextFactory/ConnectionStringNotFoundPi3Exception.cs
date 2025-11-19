// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;

namespace Pi3.App.StampaRepertori.Batch.Infrastructure.Services.OracleDbContextFactory
{

    public class ConnectionStringNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public ConnectionStringNotFoundPi3Exception(string instance)
            : base(ErrorDescriptions.ConnectionStringNotFound, null, ErrorDescriptions.ResourceManager, instance)
        {
            this.Instance = instance;
        }

        public string Instance { get; init; }

        #endregion
    }
}
