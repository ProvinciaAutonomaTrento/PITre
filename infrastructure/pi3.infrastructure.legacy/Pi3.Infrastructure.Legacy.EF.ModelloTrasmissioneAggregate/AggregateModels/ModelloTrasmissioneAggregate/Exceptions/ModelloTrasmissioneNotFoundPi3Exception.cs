// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.ModelloTrasmissioneAggregate.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.AggregateModels.ModelloTrasmissioneAggregate.Exceptions
{
    public class ModelloTrasmissioneNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public ModelloTrasmissioneNotFoundPi3Exception(string idModelloTrasmissione)
            : base(ErrorDescriptions.ModelloTrasmissioneNonTrovato, null, ErrorDescriptions.ResourceManager, idModelloTrasmissione)
        {
            IdModelloTrasmissione = idModelloTrasmissione;
        }

        public string IdModelloTrasmissione { get; init; }

        #endregion
    }
}
