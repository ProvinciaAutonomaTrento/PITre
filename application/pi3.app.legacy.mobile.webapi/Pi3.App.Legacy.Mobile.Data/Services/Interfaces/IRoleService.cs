// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
namespace Pi3.App.Legacy.Mobile.Data.Services.Interfaces;
public interface IRoleService
{
    Task<bool> CheckFunctionExistsForRoleByCode(
        long idRole,
        string functionCode,
        CancellationToken cancellationToken );
}
