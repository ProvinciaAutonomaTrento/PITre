// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using SERVICE_DTO = Pi3.App.Legacy.Mobile.Models.ServiceDtos;

namespace Pi3.App.Legacy.Mobile.Data.Services.Interfaces;
public interface IConfigurationService
{
    Task<string?> GetChiaveByIdAmmAsync( string chiave, long idAmministrazione, CancellationToken cancellationToken );

    IEnumerable<SERVICE_DTO.Istanza>? GetInstanceList();
}
