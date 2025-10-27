// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
namespace Pi3.App.InteropPitre.WebApi.Infrastructure.Services.CommonAddressBook
{
    public class SearchResponse
    {
        public int TotalePagine { get; init; }

        public int TotaleCorrispondenti { get; init; }

        public IReadOnlyList<Corrispondente>? Corrispondenti { get; init; } = null;
    }
}
