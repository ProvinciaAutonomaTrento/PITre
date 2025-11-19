// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.AggregazioneDocumentaleAggregate.Resources;
using System.Runtime.Serialization;

namespace Pi3.Infrastructure.Legacy.EF.AggregateModels.AggregazioneDocumentaleAggregate.Exceptions
{
    public class DocumentoDaRimuovereInCheckoutPi3Exception : Pi3Exception
    {
        #region Public Members

        public DocumentoDaRimuovereInCheckoutPi3Exception()
            : base(ErrorDescriptions.DocumentoDaRimuovereInCheckoutException, null, ErrorDescriptions.ResourceManager)
        {

        }

        #endregion
    }
}