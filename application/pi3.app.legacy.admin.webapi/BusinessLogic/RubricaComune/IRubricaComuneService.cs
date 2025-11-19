// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.AspNetCore.Authorization;
using Refit;
using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Refit;

namespace BusinessLogic.RubricaComune
{
    public interface IBaseRubricaComuneService
    {

        [Post("/Corrispondenti/Search")]
        Task<SearchResponse> Search(
            [Refit.Authorize("Bearer")] string token,
            [Body] SearchRequest request);

        [Get("/Corrispondenti/{id}/Emails")]
        Task<IReadOnlyList<Email>> GetEmails(
            [Refit.Authorize("Bearer")] string token,
            [AliasAs("id")] int id);

        [Delete("/Corrispondenti/{id}")]
        Task<HttpResponseMessage> Delete(
            [Refit.Authorize("Bearer")] string token,
            [AliasAs("id")] int id);
    }
}
