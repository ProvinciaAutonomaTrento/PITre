// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MODELS = Pi3.App.Legacy.Mobile.Models;

namespace Pi3.App.Legacy.Mobile.Data.Repositories.Interfaces;
public interface IRoleRepository
{
    Task<IEnumerable<MODELS.SelectTemplates.RuoloUtente>> GetRoleByIdPeopleAsync(
        long id,
        CancellationToken cancellationToken );

    Task<bool> CheckFunctionExistsForRoleByCode(
        long idRole,
        string code,
        CancellationToken cancellationToken );
}
