// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.RubricaComune
{
    public interface IRubricaComuneService
    {
        [Post("/Corrispondenti/Search")]
        Task<SearchResponse> Search(
            [Authorize("Bearer")] string token,
            [Body] SearchRequest request);

        [Get("/Corrispondenti/{id}/Emails")]
        Task<IReadOnlyList<Email>> GetEmails(
            [Authorize("Bearer")] string token,
            [AliasAs("id")] int id);
    }
}
