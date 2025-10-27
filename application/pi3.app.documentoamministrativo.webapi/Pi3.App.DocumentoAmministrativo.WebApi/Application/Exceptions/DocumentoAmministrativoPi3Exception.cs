// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.App.DocumentoAmministrativo.WebApi.Resources;
using Pi3.Core.SeedWork;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Exceptions
{

    public class DocumentoAmministrativoPi3Exception : Pi3Exception
    {
        #region Public Members

        public DocumentoAmministrativoPi3Exception(string message, System.Resources.ResourceManager resourceManager, string idDocumentoAmministrativo)
            : base(message, null, resourceManager, idDocumentoAmministrativo)
        {
            this.IdDocumentoAmministrativo = idDocumentoAmministrativo;
        }

        public string IdDocumentoAmministrativo { get; init; }

        #endregion
    }
}
