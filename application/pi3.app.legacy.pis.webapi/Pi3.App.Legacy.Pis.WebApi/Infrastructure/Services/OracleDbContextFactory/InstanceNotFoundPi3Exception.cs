// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;

namespace Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.OracleDbContextFactory
{

    public class InstanceNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public InstanceNotFoundPi3Exception()
            : base(ErrorDescriptions.InstanceNotFound, null, ErrorDescriptions.ResourceManager)
        {
        }

        #endregion
    }
}
