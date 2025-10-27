// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Exceptions
{
    public class DocumentBlobNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public DocumentBlobNotFoundPi3Exception(string idDocumentBlob)
            : base(ErrorDescriptions.DocumentBlobNotFound, null, ErrorDescriptions.ResourceManager, idDocumentBlob)
        {
            this.IdDocumentBlob = idDocumentBlob;
        }

        public string IdDocumentBlob { get; init; }

        #endregion
    }
}
