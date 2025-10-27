// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.TrasmissioneAggregate.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.AggregateModels.TrasmissioneAggregate.Exceptions
{
    public class TrasmissioneNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public TrasmissioneNotFoundPi3Exception(string idTrasmissione)
            : base(ErrorDescriptions.TrasmissioneNonTrovata, null, ErrorDescriptions.ResourceManager, idTrasmissione)
        {
            IdTrasmissione = idTrasmissione;
        }

        public string IdTrasmissione { get; init; }

        #endregion
    }
}
