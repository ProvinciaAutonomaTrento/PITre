// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using SELECT_TEMPLATES = Pi3.App.Legacy.Mobile.Models.SelectTemplates;

namespace Pi3.App.Legacy.Mobile.Data.Repositories.Interfaces;
public interface IAdministrationRepository
{
    Task<SELECT_TEMPLATES.AdministrationPasswordSettings?> GetAdministrationPasswordSettingsAsync(
        long id,
        CancellationToken cancellationToken );
}
