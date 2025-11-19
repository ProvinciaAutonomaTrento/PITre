// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.Exceptions
{
    public class IdDocPi3Exception : Pi3Exception
    {
        #region Public Members

        public IdDocPi3Exception(string message, System.Resources.ResourceManager resourceManager, IdDoc idDoc)
            : base(message, null, resourceManager, idDoc)
        {
            IdDoc = idDoc;
        }

        public IdDoc IdDoc { get; init; }

        #endregion
    }
}
