// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.NotaRFAggregate.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.AggregateModels.NotaRFAggregate.Exceptions
{
    public class NotaRFNotFoundPi3Exception : NotFoundPi3Exception
    {
        public NotaRFNotFoundPi3Exception(string idNotaRF)
            : base(ErrorDescriptions.NotaRFNotFound, null, ErrorDescriptions.ResourceManager)
        {
            IdNotaRF = idNotaRF;
        }

        public string IdNotaRF { get; init; }
    }
}
