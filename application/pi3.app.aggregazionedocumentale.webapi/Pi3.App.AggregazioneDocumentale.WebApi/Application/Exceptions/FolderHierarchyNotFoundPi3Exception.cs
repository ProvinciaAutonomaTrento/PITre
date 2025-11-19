// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.App.AggregazioneDocumentale.WebApi.Resources;
using Pi3.Core.SeedWork;

namespace Pi3.App.AggregazioneDocumentale.WebApi.Application.Exceptions
{
    public class FolderHierarchyNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public FolderHierarchyNotFoundPi3Exception(string id)
            : base(ErrorDescriptions.FolderHierarchyNotFound, null, ErrorDescriptions.ResourceManager, id)
        {
            this.Id = id;
        }

        public string Id { get; init; }

        #endregion
    }
}
