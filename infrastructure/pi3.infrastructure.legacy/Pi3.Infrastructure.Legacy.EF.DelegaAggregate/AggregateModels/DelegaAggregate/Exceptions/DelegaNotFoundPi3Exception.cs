// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.DelegaAggregate.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.AggregateModels.DelegaAggregate.Exceptions
{
    public class DelegaNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public DelegaNotFoundPi3Exception(string idDelega)
            : base(ErrorDescriptions.DelegaNotFound, null, ErrorDescriptions.ResourceManager, idDelega)
        {
            IdDelega = idDelega;
        }

        public string IdDelega { get; init; }

        #endregion
    }

}
