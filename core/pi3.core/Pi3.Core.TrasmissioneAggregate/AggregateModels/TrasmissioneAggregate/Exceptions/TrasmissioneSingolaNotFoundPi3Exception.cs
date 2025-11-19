// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.AggregateModels.TrasmissioneAggregate.Resources;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.TrasmissioneAggregate.Exceptions
{
    public class TrasmissioneSingolaNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public TrasmissioneSingolaNotFoundPi3Exception(string idTrasmissioneSingola)
            : base(ErrorDescriptions.TrasmissioneSingolaNonTrovata, null, ErrorDescriptions.ResourceManager, idTrasmissioneSingola)
        {
            IdTrasmissioneSingola = idTrasmissioneSingola;
        }

        public string IdTrasmissioneSingola { get; init; }

        #endregion
    }
}
