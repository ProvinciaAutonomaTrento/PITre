// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.Resources;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.Exceptions
{
    public class TipoAggregazioneNonDefinitaPi3Exception : Pi3Exception
    {
        #region Public Members

        public TipoAggregazioneNonDefinitaPi3Exception()
            : base(ErrorDescriptions.TipoAggregazioneNonDefinita, null, ErrorDescriptions.ResourceManager)
        {
        }

        #endregion
    }
}
