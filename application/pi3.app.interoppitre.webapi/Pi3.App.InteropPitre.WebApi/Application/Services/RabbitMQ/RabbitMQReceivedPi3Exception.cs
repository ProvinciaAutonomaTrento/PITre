// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.App.InteropPitre.WebApi.Resources;
using Pi3.Core.SeedWork;

namespace Pi3.App.InteropPitre.WebApi.Application.Services.RabbitMQ
{
    public class RabbitMQReceivedPi3Exception : Pi3Exception
    {
        #region Public Members

        public RabbitMQReceivedPi3Exception(Exception? innerException)
            : base(ErrorDescriptions.UnhandledError, ErrorDescriptions.ResourceManager, innerException)
        {
        }

        #endregion
    }
}
