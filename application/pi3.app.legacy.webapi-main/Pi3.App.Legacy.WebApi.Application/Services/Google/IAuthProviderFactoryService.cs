// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Google.Apis.Auth.AspNetCore3;

namespace Pi3.App.Legacy.WebApi.Application.Services.Google
{
    public interface IAuthProviderFactoryService
    {
        AuthProvider Create(string instance, string codiceAmministrazione, string codiceRegistro);
    }
}