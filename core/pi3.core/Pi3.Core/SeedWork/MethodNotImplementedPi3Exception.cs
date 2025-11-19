// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.SeedWork
{
    public class MethodNotImplementedPi3Exception : Pi3Exception
    {
        #region Public Members

        public MethodNotImplementedPi3Exception(string methodName)
            : base(ErrorDescriptions.MethodNotImplemented, ErrorDescriptions.ResourceManager, methodName)
        {
            this.MethodName = methodName;
        }

        public string MethodName { get; init; }

        #endregion
    }
}
