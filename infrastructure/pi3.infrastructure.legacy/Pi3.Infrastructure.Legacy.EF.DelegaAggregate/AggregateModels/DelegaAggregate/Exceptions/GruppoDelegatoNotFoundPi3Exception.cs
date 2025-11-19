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
    public class GruppoDelegatoNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public GruppoDelegatoNotFoundPi3Exception(string idGruppoDelegato)
            : base(ErrorDescriptions.GruppoDelegatoNotFound, null, ErrorDescriptions.ResourceManager, idGruppoDelegato)
        {
            IdGruppoDelegato = idGruppoDelegato;
        }

        public string IdGruppoDelegato { get; init; }

        #endregion
    }
}
