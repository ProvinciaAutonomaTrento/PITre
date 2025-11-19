// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
namespace Pi3.App.Legacy.Mobile.Data.Repositories.Interfaces;
public interface IConfigurationRepository
{
    Task<string?> GetChiaveByIdAmmAsync( string chiave, long idAmministrazione, CancellationToken cancellationToken );
    Task<string> GetConfigurationValue( string key );
}
