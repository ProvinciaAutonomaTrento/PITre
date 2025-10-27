// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Infrastructure.Services.RabbitMQ
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
