// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Exceptions;

public class ModelCodeNotFoundPi3Exception : NotFoundPi3Exception
{
    #region Public Members

    public ModelCodeNotFoundPi3Exception(string oggettoCustom)
        : base(ErrorDescriptions.CodiceModelloNotFound, null, ErrorDescriptions.ResourceManager, oggettoCustom)
    {
    }

    #endregion
}
