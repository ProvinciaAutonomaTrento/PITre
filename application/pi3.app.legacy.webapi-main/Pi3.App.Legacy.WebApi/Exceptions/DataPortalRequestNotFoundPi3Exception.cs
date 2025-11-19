// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using Pi3.App.Legacy.WebApi.Resources;

namespace Pi3.App.Legacy.WebApi.Exceptions
{
    public class DataPortalRequestNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public DataPortalRequestNotFoundPi3Exception(string requestType)
            : base(ErrorDescriptions.DataPortalRequestNotFound, null, ErrorDescriptions.ResourceManager, requestType)
        {
            this.RequestType = requestType;
        }

        public string RequestType { get; init; }

        #endregion
    }
}
