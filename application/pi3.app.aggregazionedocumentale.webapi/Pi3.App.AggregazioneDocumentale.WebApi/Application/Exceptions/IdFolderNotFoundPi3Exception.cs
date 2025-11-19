// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.App.AggregazioneDocumentale.WebApi.Resources;
using Pi3.Core.SeedWork;

namespace Pi3.App.AggregazioneDocumentale.WebApi.Application.Exceptions
{
    public class IdFolderNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public IdFolderNotFoundPi3Exception(string idFolder)
            : base(ErrorDescriptions.FolderNotFound, null, ErrorDescriptions.ResourceManager, idFolder)
        {
            this.IdFolder = idFolder;
        }

        public string IdFolder { get; init; }

        #endregion
    }
}
