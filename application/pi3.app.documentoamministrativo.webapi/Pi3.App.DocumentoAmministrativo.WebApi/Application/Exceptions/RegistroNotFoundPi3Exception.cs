// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Exceptions;

public class RegistroNotFoundPi3Exception : NotFoundPi3Exception
{
    #region Public Members

    public RegistroNotFoundPi3Exception(string codiceRegistro)
        : base(ErrorDescriptions.RegistroNotFound, null, ErrorDescriptions.ResourceManager, codiceRegistro)
    {
        this.CodiceRegistro = codiceRegistro;
    }

    public string CodiceRegistro { get; init; }

    #endregion
}
