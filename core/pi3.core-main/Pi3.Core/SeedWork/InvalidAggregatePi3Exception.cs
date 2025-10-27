// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.SeedWork
{
    public class InvalidAggregatePi3Exception : Pi3Exception
    {
        #region Public Members

        public InvalidAggregatePi3Exception(IEnumerable<Pi3Exception> errors)
            : base(ErrorDescriptions.InvalidElement, null, ErrorDescriptions.ResourceManager)
        {
            Errors = errors;
        }

        public IEnumerable<Pi3Exception> Errors
        {
            get;
            init;
        }

        #endregion
    }
}
