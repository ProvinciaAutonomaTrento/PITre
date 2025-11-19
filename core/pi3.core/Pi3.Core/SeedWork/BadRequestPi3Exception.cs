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
    public class BadRequestPi3Exception : Pi3Exception
    {
        public BadRequestPi3Exception() : base()
        {
        }

        public BadRequestPi3Exception(Exception innerException) : base(null, innerException)
        {
        }

        public BadRequestPi3Exception(string message, System.Resources.ResourceManager resourceManager, params object[] messageParameters) : base(message, resourceManager, messageParameters)
        {
        }

        public BadRequestPi3Exception(string message, System.Resources.ResourceManager resourceManager, Exception? innerException, params object[] messageParameters) : base(message, resourceManager, innerException, messageParameters)
        {
        }

        public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
    }
}
