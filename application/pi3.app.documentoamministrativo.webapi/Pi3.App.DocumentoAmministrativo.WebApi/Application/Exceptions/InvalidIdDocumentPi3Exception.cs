// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Exceptions;

public class InvalidIdDocumentPi3Exception : NotFoundPi3Exception
{
    #region Public Members

    public InvalidIdDocumentPi3Exception(string idDocumentoAmministrativo)
        : base(ErrorDescriptions.IdDocumentoAmministrativoNonValido, null, ErrorDescriptions.ResourceManager, idDocumentoAmministrativo)
    {
        this.IdDocumentoAmministrativo = idDocumentoAmministrativo;
    }

    public string IdDocumentoAmministrativo { get; init; }

    #endregion
}
