// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;

namespace Pi3.App.AggregazioneDocumentale.WebApi.Application.Exceptions
{
    public class TrasmissionePi3Exception : Pi3Exception
    {
        #region Public Members

        public TrasmissionePi3Exception(string message, System.Resources.ResourceManager resourceManager, string idTrasmissione)
            : base(message, null, resourceManager, idTrasmissione)
        {
            this.IdTrasmissione = idTrasmissione;
        }

        public string IdTrasmissione { get; init; }

        #endregion
    }
}
