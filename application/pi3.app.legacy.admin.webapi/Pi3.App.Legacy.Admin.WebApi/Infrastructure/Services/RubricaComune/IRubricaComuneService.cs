// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using BusinessLogic.RubricaComune;
using DocsPaVO.rubrica;
using DocsPaVO.RubricaComune;
using Pi3.Core.Extensions;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.Admin.WebApi.Infrastructure.Services.RubricaComune
{

    public interface IRubricaComuneService : IBaseRubricaComuneService
    {

        [Post("/Corrispondenti/Search")]
        Task<SearchResponse> Search(
            [Authorize("Bearer")] string token,
            [Body] SearchRequest request);

        [Get("/Corrispondenti/{id}/Emails")]
        Task<IReadOnlyList<Email>> GetEmails(
            [Authorize("Bearer")] string token,
            [AliasAs("id")] int id);

        [Delete("/Corrispondenti/{id}")]
        Task<HttpResponseMessage> Delete(
            [Authorize("Bearer")] string token,
            [AliasAs("id")] int id);
    }


}


