// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.AggregazioneDocumentaleAggregate.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.AggregateModels.AggregazioneDocumentaleAggregate.Exceptions
{
    public class TitolarioNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public TitolarioNotFoundPi3Exception(string idTitolario)
            : base(ErrorDescriptions.TitolarioNotFound, ErrorDescriptions.ResourceManager, idTitolario)
        {
            IdTitolario = idTitolario;
        }

        public string IdTitolario { get; init; }

        #endregion
    }
}
