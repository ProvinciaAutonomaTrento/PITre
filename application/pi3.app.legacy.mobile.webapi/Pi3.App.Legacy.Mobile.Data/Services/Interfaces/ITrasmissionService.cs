// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
namespace Pi3.App.Legacy.Mobile.Data.Services.Interfaces;
using SERVICE_DTO = Pi3.App.Legacy.Mobile.Models.ServiceDtos;

public interface ITrasmissionService
{
    Task<SERVICE_DTO.Trasmissione> GetDettagliTrasmissioneUtenteAsync(
        long id,
        CancellationToken cancellationToken = default );
}
