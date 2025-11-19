// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.ModelloTrasmissioneAggregate.Exceptions
{
    public class RagioneTrasmissionePi3Exception : Pi3Exception
    {
        #region Public Members

        public RagioneTrasmissionePi3Exception(
            string idRagioneTrasmissione, string message, System.Resources.ResourceManager resourceManager, params object[] messageParameters)
            : base(message, resourceManager, messageParameters)
        {
            IdRagioneTrasmissione = idRagioneTrasmissione;
        }

        public string IdRagioneTrasmissione { get; init; }

        #endregion
    }
}
