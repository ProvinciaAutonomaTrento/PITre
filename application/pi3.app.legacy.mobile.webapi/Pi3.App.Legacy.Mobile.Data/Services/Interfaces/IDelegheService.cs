// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using SERVICE_DTO = Pi3.App.Legacy.Mobile.Models.ServiceDtos;
using SERVICE_REQUEST = Pi3.App.Legacy.Mobile.Models.ServiceRequests;


namespace Pi3.App.Legacy.Mobile.Data.Services.Interfaces;
public interface IDelegheService
{
    Task<IEnumerable<SERVICE_DTO.Delega>> GetDeleghe( string stato, string tipo, CancellationToken cancellationToken );
    Task<bool> CreaDelega(SERVICE_REQUEST.CreaDelegaRequest request, CancellationToken cancellationToken );
    Task<bool> RimuoviDeleghe( IEnumerable<SERVICE_DTO.Delega> deleghe );
}
