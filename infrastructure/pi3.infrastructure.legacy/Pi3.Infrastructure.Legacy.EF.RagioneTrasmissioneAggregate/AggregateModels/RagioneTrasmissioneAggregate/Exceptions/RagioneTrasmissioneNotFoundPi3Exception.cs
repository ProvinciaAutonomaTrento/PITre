// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.RagioneTrasmissioneAggregate.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.RagioneTrasmissioneAggregate.AggregateModels.RagioneTrasmissioneAggregate.Exceptions
{
    public class RagioneTrasmissioneNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public RagioneTrasmissioneNotFoundPi3Exception(string idRagioneTrasmissione)
            : base(ErrorDescriptions.RagioneTrasmissioneNotFound, null, ErrorDescriptions.ResourceManager, idRagioneTrasmissione)
        {
            this.IdRagioneTrasmissione = idRagioneTrasmissione;
        }

        public string IdRagioneTrasmissione { get; init; }

        #endregion
    }
}
