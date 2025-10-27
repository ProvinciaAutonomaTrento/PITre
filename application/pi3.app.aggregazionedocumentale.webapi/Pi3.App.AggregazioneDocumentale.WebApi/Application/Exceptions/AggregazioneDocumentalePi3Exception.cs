// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;

namespace Pi3.App.AggregazioneDocumentale.WebApi.Application.Exceptions
{
    public class AggregazioneDocumentalePi3Exception : Pi3Exception
    {
        #region Public Members

        public AggregazioneDocumentalePi3Exception(string message, System.Resources.ResourceManager resourceManager, string idAggregazioneDocumentale)
            : base(message, null, resourceManager, idAggregazioneDocumentale)
        {
            this.IdAggregazioneDocumentale = idAggregazioneDocumentale;
        }

        public string IdAggregazioneDocumentale { get; init; }

        #endregion
    }
}
