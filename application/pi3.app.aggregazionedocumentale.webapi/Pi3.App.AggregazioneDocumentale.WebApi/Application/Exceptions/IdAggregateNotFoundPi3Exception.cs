// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.App.AggregazioneDocumentale.WebApi.Resources;
using Pi3.Core.SeedWork;

namespace Pi3.App.AggregazioneDocumentale.WebApi.Application.Exceptions
{
    public class IdAggregateNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public IdAggregateNotFoundPi3Exception(string idAggregate)
            : base(ErrorDescriptions.IdAggregateNotFound, null, ErrorDescriptions.ResourceManager, idAggregate)
        {
            this.IdAggregate = idAggregate;
        }

        public string IdAggregate { get; init; }

        #endregion
    }
}
