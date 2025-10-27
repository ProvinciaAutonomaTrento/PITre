// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;

namespace Pi3.App.Legacy.Mobile.WebApi.Services.RubricaComune
{
    public class SearchResponse : ValueObject
    {
        public int TotalePagine { get; init; }

        public int TotaleCorrispondenti { get; init; }

        public IReadOnlyList<Corrispondente>? Corrispondenti { get; init; } = null;

    }
}
