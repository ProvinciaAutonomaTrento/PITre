// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.TrasmissioneAggregate.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.AggregateModels.TrasmissioneAggregate.Exceptions
{
    public class AutoreTrasmissioneNonValidoPi3Exception : Pi3Exception
    {
        #region Public Members

        public AutoreTrasmissioneNonValidoPi3Exception(string idUtente, string idGruppo)
            : base(ErrorDescriptions.TrasmissioneNonTrovata, null, ErrorDescriptions.ResourceManager)
        {
            IdUtente = idUtente;
            IdGruppo = idGruppo;
        }

        public string IdUtente { get; init; }
        public string IdGruppo { get; init; }

        #endregion
    }
}
