// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using Pi3.Core.Test.OrderAggregate.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Test.OrderAggregate.Exceptions
{

    public class InvalidDiscountPi3Exception : Pi3Exception
    {
        #region Public Members

        public InvalidDiscountPi3Exception()
            : base(ErrorDescriptions.InvalidDiscount, null, ErrorDescriptions.ResourceManager)
        {
        }

        #endregion
    }
}
