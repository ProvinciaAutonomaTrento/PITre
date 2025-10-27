// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pi3.Core.SeedWork;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.KeywordAggregate.Resources;

namespace Pi3.Infrastructure.Legacy.EF.AggregateModels.KeywordAggregate.Exceptions
{
    internal class KeywordAlreadyExistPi3Exception : Pi3Exception
    {
        public KeywordAlreadyExistPi3Exception()
        : base(ErrorDescriptions.KeywordAlreadyExist, null, ErrorDescriptions.ResourceManager)
        {
        }
    }
}
