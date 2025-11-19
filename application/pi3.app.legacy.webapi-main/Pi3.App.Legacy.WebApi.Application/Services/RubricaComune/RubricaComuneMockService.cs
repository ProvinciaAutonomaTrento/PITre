// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Services.RubricaComune
{
    public class RubricaComuneMockService : IRubricaComuneService
    {
        public async Task<IReadOnlyList<Email>> GetEmails(string authorization, int id)
        {
            return new List<Email>()
            {
                new Email()
                {
                    Indirizzo = "mockservice@gmail.com"
                }
            }.AsReadOnly();
        }

        public async Task<SearchResponse> Search(string authorization, SearchRequest request)
        {
            return new SearchResponse()
            {
                Corrispondenti = new List<Corrispondente>().AsReadOnly(),
                TotaleCorrispondenti = 0,
                TotalePagine = 0
            };
        }
    }
}
