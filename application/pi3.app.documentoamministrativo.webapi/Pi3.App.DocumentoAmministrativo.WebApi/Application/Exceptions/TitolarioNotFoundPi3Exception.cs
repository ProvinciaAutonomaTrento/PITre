// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Exceptions;

public class TitolarioNotFoundPi3Exception : NotFoundPi3Exception
{
    #region Public Members

    public TitolarioNotFoundPi3Exception(long idTitolario)
        : base(ErrorDescriptions.TitolarioNotFound, null, ErrorDescriptions.ResourceManager, idTitolario.ToString())
    {
        this.IdTitolario = idTitolario;
    }

    public long IdTitolario { get; init; }

    #endregion
}
