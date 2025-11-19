// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.SeedWork
{
    public abstract class NotFoundPi3Exception : Pi3Exception
    {
        public NotFoundPi3Exception()
        {
        }

        public NotFoundPi3Exception(string? message)
            : base(message)
        {
        }

        public NotFoundPi3Exception(string? message, Exception? innerException)
            : base(message, innerException)
        {
        }

        public NotFoundPi3Exception(string message, System.Resources.ResourceManager resourceManager, params object[] messageParameters)
            : this(message, null, resourceManager, messageParameters)
        {
        }

        public NotFoundPi3Exception(string message, Exception? innerException, System.Resources.ResourceManager resourceManager, params object[] messageParameters)
            : base(string.Format(message, messageParameters), innerException)
        {
            MapError(message, resourceManager, messageParameters);
        }

        public override HttpStatusCode StatusCode => HttpStatusCode.NotFound;
    }
}
