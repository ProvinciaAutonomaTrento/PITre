// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.AggregateModels.UOCorrispondenteAggregate.Resources;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.UOCorrispondenteAggregate.Exceptions
{
    public class CodiceFiscaleNotValidPi3Exception : Pi3Exception
    {
        #region Public Members

        public CodiceFiscaleNotValidPi3Exception()
            : base(ErrorDescriptions.CodiceFiscaleNotValid, ErrorDescriptions.ResourceManager)
        {
        }

        #endregion
    }

    public class PartitaIvaNotValidPi3Exception : Pi3Exception
    {
        #region Public Members

        public PartitaIvaNotValidPi3Exception()
            : base(ErrorDescriptions.PartitaIvaNotValid, ErrorDescriptions.ResourceManager)
        {
        }

        #endregion
    }
}
