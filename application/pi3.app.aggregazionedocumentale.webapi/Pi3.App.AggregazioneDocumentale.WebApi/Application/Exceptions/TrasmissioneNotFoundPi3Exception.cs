// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.App.AggregazioneDocumentale.WebApi.Resources;
using Pi3.Core.SeedWork;

namespace Pi3.App.AggregazioneDocumentale.WebApi.Application.Exceptions
{
    public class TrasmissioneNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public TrasmissioneNotFoundPi3Exception()
            : base(ErrorDescriptions.TrasmisssioneNotFound, null, ErrorDescriptions.ResourceManager)
        {
        }

        #endregion
    }
}
