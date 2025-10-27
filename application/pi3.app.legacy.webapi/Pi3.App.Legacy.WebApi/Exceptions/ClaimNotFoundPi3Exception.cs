// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using Pi3.App.Legacy.WebApi.Resources;

namespace Pi3.App.Legacy.WebApi.Exceptions
{
    public class ClaimNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public ClaimNotFoundPi3Exception(string type)
            : base(ErrorDescriptions.ClaimNotFound, null, ErrorDescriptions.ResourceManager, type)
        {
            this.Type = type;
        }

        public string Type { get; init; }

        #endregion
    }
}
