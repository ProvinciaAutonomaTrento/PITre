// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.App.Legacy.Mobile.Data.Repositories.Interfaces;
using Pi3.App.Legacy.Mobile.Data.Services.Interfaces;

namespace Pi3.App.Legacy.Mobile.Data.Services;
public class RoleService(IRoleRepository roleRepository) : IRoleService
{
    readonly IRoleRepository _roleRepository = roleRepository;

    public async Task<bool> CheckFunctionExistsForRoleByCode(
        long idRole,
        string functionCode,
        CancellationToken cancellationToken )
    {
        return await this._roleRepository.CheckFunctionExistsForRoleByCode(idRole, functionCode, cancellationToken);
    }
}
