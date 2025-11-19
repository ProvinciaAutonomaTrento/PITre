// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;

namespace Pi3.App.Legacy.WebApi.Application.Services.OracleDbContextFactory
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
