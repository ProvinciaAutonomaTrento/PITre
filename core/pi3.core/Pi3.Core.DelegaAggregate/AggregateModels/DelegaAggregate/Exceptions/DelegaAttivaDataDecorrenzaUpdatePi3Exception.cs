// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.AggregateModels.DelegaAggregate.Resources;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.DelegaAggregate.Exceptions
{
    public class DelegaAttivaDataDecorrenzaUpdatePi3Exception : Pi3Exception
    {
        #region Public Members

        public DelegaAttivaDataDecorrenzaUpdatePi3Exception()
            : base(ErrorDescriptions.DelegaAttivaDataDecorrenzaUpdate, null, ErrorDescriptions.ResourceManager)
        {
        }

        #endregion
    }
}
