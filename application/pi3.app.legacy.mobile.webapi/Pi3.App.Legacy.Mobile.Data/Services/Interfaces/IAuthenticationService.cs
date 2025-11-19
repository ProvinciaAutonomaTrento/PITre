// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.App.Legacy.Mobile.Models.ServiceRequests;
using SERVICE_DTO = Pi3.App.Legacy.Mobile.Models.ServiceDtos;

namespace Pi3.App.Legacy.Mobile.Data.Services.Interfaces;
public interface IAuthenticationService
{
    Task<SERVICE_DTO.AuthenticationResult> AuthenticateUser( AuthenticateUserRequest authenticateUserRequest, CancellationToken cancellationToken );
}
