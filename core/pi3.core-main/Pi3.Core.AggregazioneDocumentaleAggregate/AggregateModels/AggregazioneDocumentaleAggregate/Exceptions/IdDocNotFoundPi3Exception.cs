// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.Resources;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.Exceptions
{
    public class IdDocNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public IdDocNotFoundPi3Exception(IdDoc idDoc)
            : base(ErrorDescriptions.DocumentoNonTrovato, null, ErrorDescriptions.ResourceManager, null)
        {
            IdDoc = idDoc;
        }

        public IdDoc IdDoc { get; init; }

        #endregion
    }
}
