// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Refit;

namespace Pi3.App.InteropPitre.WebApi.Infrastructure.Services.CommonAddressBook
{
    public interface ICorrispondenti
    {
        [Post("/Corrispondenti/Search")]
        Task<SearchResponse> Search(
            [Authorize("Bearer")] string token,
            [Body]SearchRequest request);
    }
}
