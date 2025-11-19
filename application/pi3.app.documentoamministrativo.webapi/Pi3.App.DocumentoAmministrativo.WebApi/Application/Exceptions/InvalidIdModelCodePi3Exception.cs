// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Exceptions;

public class InvalidIdModelCodePi3Exception : NotFoundPi3Exception
{
    #region Public Members

    public InvalidIdModelCodePi3Exception(string codiceModello)
        : base(ErrorDescriptions.CodiceModelloNonValido, null, ErrorDescriptions.ResourceManager, codiceModello)
    {
        this.CodiceModello = codiceModello;
    }

    public string CodiceModello { get; init; }

    #endregion
}
