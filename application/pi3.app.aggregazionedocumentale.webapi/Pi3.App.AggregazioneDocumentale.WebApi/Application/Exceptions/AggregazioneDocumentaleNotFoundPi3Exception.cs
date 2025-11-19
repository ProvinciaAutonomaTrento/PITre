// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.App.AggregazioneDocumentale.WebApi.Resources;
using Pi3.Core.SeedWork;

namespace Pi3.App.AggregazioneDocumentale.WebApi.Application.Exceptions
{
    public class AggregazioneDocumentaleNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public AggregazioneDocumentaleNotFoundPi3Exception(string idAggregazioneDocumentale)
            : base(ErrorDescriptions.AggregazioneDocumentaleNotFound, null, ErrorDescriptions.ResourceManager, idAggregazioneDocumentale)
        {
            this.IdAggregazioneDocumentale = idAggregazioneDocumentale;
        }

        public string IdAggregazioneDocumentale { get; init; }

        #endregion
    }
}
