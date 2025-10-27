// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate;
using Pi3.Core.SeedWork;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.AggregazioneDocumentaleAggregate.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.AggregateModels.AggregazioneDocumentaleAggregate.Exceptions
{
    public class CampoProfilatoFascicoloPresentePi3Exception : Pi3Exception
    {
        #region Public Members

        public CampoProfilatoFascicoloPresentePi3Exception(string idAggregazioneDocumentale)
            : base(ErrorDescriptions.CampoProfilatoFascicoloPresente, null, ErrorDescriptions.ResourceManager, idAggregazioneDocumentale)
        {
            IdAggregazioneDocumentale = idAggregazioneDocumentale;
        }

        public string IdAggregazioneDocumentale { get; init; }

        #endregion
    }
}
