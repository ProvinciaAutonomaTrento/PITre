// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.App.AggregazioneDocumentale.WebApi.Resources;
using Pi3.Core.SeedWork;

namespace Pi3.App.AggregazioneDocumentale.WebApi.Application.Exceptions
{
    public class IdDocNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public IdDocNotFoundPi3Exception(string idDoc)
            : base(ErrorDescriptions.IdDocNotFound, null, ErrorDescriptions.ResourceManager, idDoc)
        {
            this.IdDoc = idDoc;
        }

        public string IdDoc { get; init; }

        #endregion
    }
}
