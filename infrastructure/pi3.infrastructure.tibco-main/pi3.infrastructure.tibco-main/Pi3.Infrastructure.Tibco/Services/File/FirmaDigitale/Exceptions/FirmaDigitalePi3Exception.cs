// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Tibco.Services.File.FirmaDigitale.Exceptions
{
    public class FirmaDigitalePi3Exception : Pi3Exception
    {
        #region Public Members

        public FirmaDigitalePi3Exception(string message, System.Resources.ResourceManager resourceManager, params object[] messageParameters)
            : base(message, resourceManager, null, messageParameters)
        {
        }

        public FirmaDigitalePi3Exception(string message, System.Resources.ResourceManager resourceManager, Exception? innerException, params object[] messageParameters)
            : base(message, resourceManager, innerException, messageParameters)
        {
        }

        #endregion
    }

}
