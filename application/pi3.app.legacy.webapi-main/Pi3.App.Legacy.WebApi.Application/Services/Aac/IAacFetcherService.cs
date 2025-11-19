// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.App.Legacy.WebApi.Application.Services.Aac.Domain;
using Refit;

namespace Pi3.App.Legacy.WebApi.Application.Services.Aac
{
    public interface IAacFetcherService
    {
        [Post("/oauth/token")]
        Task<AacResponse> GetToken([Body(BodySerializationMethod.UrlEncoded)] AacRequestBody data);
    }
}
