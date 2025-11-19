// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Refit;
using System.DirectoryServices.Protocols;

namespace Pi3.App.Legacy.Mobile.WebApi.Services.RubricaComune
{

    public interface IRubricaComuneService
    {
        [Post("/Corrispondenti/Search")]
        Task<SearchResponse> Search(
            [Header("Authorization")] string authorization,
            //[Authorize("Bearer")] string token,
            [Body] SearchRequest request);

        [Get("/Corrispondenti/{id}/Emails")]
        Task<IReadOnlyList<Email>> GetEmails(
            [Header("Authorization")] string authorization,
            //[Authorize("Bearer")] string token,
            [AliasAs("id")] int id);
    }
}
