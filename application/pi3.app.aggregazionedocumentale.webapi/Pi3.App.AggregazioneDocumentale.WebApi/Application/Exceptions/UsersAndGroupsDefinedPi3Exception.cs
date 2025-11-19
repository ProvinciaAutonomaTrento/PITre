// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.App.AggregazioneDocumentale.WebApi.Resources;
using Pi3.Core.SeedWork;

namespace Pi3.App.AggregazioneDocumentale.WebApi.Application.Exceptions
{
    public class UsersAndGroupsDefinedPi3Exception : Pi3Exception
    {
        #region Public Members

        public UsersAndGroupsDefinedPi3Exception(string idTrasmissione)
             : base(ErrorDescriptions.UsersAndGroupsDefined, null, ErrorDescriptions.ResourceManager, idTrasmissione)
        {
            this.IdTrasmissione = idTrasmissione;
        }

        public string IdTrasmissione { get; init; }

        #endregion
    }
}
