// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Exceptions;

public class TipoCampoNotFoundPi3Exception : NotFoundPi3Exception
{
    #region Public Members

    public TipoCampoNotFoundPi3Exception(string tipoCampo)
        : base(ErrorDescriptions.TipoCampoNotFound, null, ErrorDescriptions.ResourceManager, tipoCampo)
    {
    }

    #endregion
}
