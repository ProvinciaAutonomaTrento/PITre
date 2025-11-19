// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Pi3.Core.SeedWork;

namespace Pi3.App.RubricaComune.WebApi.Application.Queries.Corrispondenti.Search
{
    public class SearchIpaResponse : ValueObject
    {
        public int TotalePagine { get; init; }

        public int TotaleCorrispondenti { get; init; }

        public IReadOnlyList<Corrispondente>? Corrispondenti { get; init; } = null;
    }
}
