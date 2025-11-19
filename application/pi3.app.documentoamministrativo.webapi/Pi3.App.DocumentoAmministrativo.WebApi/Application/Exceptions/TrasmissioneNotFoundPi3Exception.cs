// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Exceptions
{
    public class TrasmissioneNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public TrasmissioneNotFoundPi3Exception(string idTrasmissione)
            : base(ErrorDescriptions.TrasmissioneNotFound, null, ErrorDescriptions.ResourceManager, idTrasmissione)
        {
            this.IdTrasmissione = idTrasmissione;
        }

        public string IdTrasmissione { get; init; }

        #endregion
    }
}
