// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Exceptions;

public class InvalidModelCodePi3Exception : NotFoundPi3Exception
{
    #region Public Members

    public InvalidModelCodePi3Exception(string codiceModello)
        : base(ErrorDescriptions.CodiceModelloNonDocumento, null, ErrorDescriptions.ResourceManager, codiceModello)
    {
        _CodiceModello = codiceModello;
    }

    public string _CodiceModello { get; set; }

    #endregion
}
