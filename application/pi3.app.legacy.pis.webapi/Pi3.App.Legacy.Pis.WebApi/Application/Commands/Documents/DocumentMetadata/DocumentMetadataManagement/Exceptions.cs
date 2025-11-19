// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.DocumentMetadata.DocumentMetadataManagement;
using Pi3.Core.SeedWork;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.DocumentMetadata.DocumentMetadataManagement
{
    public class CodiceEventoNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public CodiceEventoNotFoundPi3Exception(string code)
            : base(ErrorDescriptions.CodiceEventoNotFound, null, ErrorDescriptions.ResourceManager, code)
        { }
        #endregion
    }
}
