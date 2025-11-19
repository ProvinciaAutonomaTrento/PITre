// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.NotaAggregate.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.AggregateModels.NotaAggregate.Exceptions
{
    public class NotaNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public NotaNotFoundPi3Exception(string idNota)
            : base(ErrorDescriptions.NotaNonTrovata, null, ErrorDescriptions.ResourceManager, idNota)
        {
            IdNota = idNota;
        }

        public string IdNota { get; init; }

        #endregion
    }
}
