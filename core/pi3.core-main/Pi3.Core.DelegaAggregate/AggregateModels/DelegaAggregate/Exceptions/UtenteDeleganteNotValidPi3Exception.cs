// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.AggregateModels.DelegaAggregate.Resources;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.DelegaAggregate.Exceptions
{
    public class UtenteDeleganteNotValidPi3Exception : Pi3Exception
    {
        #region Public Members

        public UtenteDeleganteNotValidPi3Exception()
            : base(ErrorDescriptions.UtenteDeleganteNotValid, null, ErrorDescriptions.ResourceManager)
        {
        }

        #endregion
    }
}
