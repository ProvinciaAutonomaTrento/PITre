// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Exceptions;

public class UsersAndGroupsDefinedPi3Exception : NotFoundPi3Exception
{
    #region Public Members

    public UsersAndGroupsDefinedPi3Exception()
        : base(ErrorDescriptions.UsersAndGroupsDefined, null, ErrorDescriptions.ResourceManager)
    {
    }

    #endregion
}
