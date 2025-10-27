// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.SeedWork
{
    public class NotSupportedPi3Exception : Pi3Exception
    {
        #region Public Members

        public NotSupportedPi3Exception()
        {
        }

        public NotSupportedPi3Exception(string? message)
            : base(message)
        {
        }

        public NotSupportedPi3Exception(string? message, Exception? innerException)
            : base(message, innerException)
        {
        }

        public NotSupportedPi3Exception(string message, System.Resources.ResourceManager resourceManager, params object[] messageParameters)
            : this(message, null, resourceManager, messageParameters)
        {
        }

        public NotSupportedPi3Exception(string message, Exception? innerException, System.Resources.ResourceManager resourceManager, params object[] messageParameters)
            : base(string.Format(message, messageParameters), innerException)
        {
            MapError(message, resourceManager, messageParameters);
        }

        #endregion
    }
}
