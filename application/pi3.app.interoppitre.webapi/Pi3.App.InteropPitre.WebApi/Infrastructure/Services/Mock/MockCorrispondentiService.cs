// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.App.InteropPitre.WebApi.Infrastructure.Services.CommonAddressBook;
using Refit;

namespace Pi3.App.InteropPitre.WebApi.Infrastructure.Services.Mock
{
    public class MockCorrispondentiService : ICorrispondenti
    {
        public async Task<SearchResponse> Search([Authorize("Bearer")] string token, [Body] SearchRequest request)
        {
            return new SearchResponse
            {
                TotaleCorrispondenti = 1,
                TotalePagine = 1,
                Corrispondenti = new List<Corrispondente> { new Corrispondente
                {
                    Codice = request.CriteriRicerca.First().Valore,
                    Denominazione = "CORRISPONDENTE_TEST",
                    AOO = "TEST",
                    Amministrazione = "TEST",
                    UrlApiInteroperabilita = "http://localhost/test",
                    CodiceFiscale = "TSTTST24A11L378V",
                    PartitaIva = "54144780340",
                    Indirizzo = "TEST 1234",
                    CAP = "00000",
                    Citta = "TEST",

                } }
            };
        }
    }
}
