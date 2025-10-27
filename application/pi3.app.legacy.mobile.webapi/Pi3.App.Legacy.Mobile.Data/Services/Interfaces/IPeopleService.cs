// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using SERVICE_DTO = Pi3.App.Legacy.Mobile.Models.ServiceDtos;
using SERVICE_REQUESTS = Pi3.App.Legacy.Mobile.Models.ServiceRequests;

namespace Pi3.App.Legacy.Mobile.Data.Services.Interfaces;
public interface IPeopleService
{
    Task<SERVICE_DTO.UserClaims?> GetUserClaims(
        SERVICE_REQUESTS.GetUserClaimsRequest request,
        CancellationToken cancellationToken );

    Task<IEnumerable<SERVICE_DTO.GetRuoliUtenteResult>?> GetRuoliUtenteAsync(
        long id,
        CancellationToken cancellationToken );

}
