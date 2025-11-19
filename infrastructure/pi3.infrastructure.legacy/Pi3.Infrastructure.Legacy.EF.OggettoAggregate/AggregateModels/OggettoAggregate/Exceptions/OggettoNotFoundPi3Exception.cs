// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.OggettoAggregate.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.AggregateModels.OggettoAggregate.Exceptions
{
    public class OggettoNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public OggettoNotFoundPi3Exception(string idOggetto)
            : base(ErrorDescriptions.OggettoNonTrovato, null, ErrorDescriptions.ResourceManager, idOggetto)
        {
            IdOggetto = idOggetto;
        }

        public string IdOggetto { get; init; }

        #endregion
    }
}
