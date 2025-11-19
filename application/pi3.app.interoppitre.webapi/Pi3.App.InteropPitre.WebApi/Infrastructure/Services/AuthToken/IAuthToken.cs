// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
//using Refit;

namespace Pi3.App.InteropPitre.WebApi.Infrastructure.Services.AuthToken
{
    public interface IAuthToken
    {
        //[Post("/token")]
        //Task<TokenResponse> GetAuthTokenAsync([Body(BodySerializationMethod.UrlEncoded)] TokenRequest request);

        string GetAuthenticationToken();
    }
}
