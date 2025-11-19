// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Pi3.Core.SeedWork;
using System.ComponentModel.DataAnnotations;

namespace Pi3.App.RubricaComune.WebApi.Application.Queries.Corrispondenti.Search
{
    public class SearchIpaRequest : ValueObject, IRequest<SearchIpaResponse>
    {
        [Required]
        [Range(1, Int32.MaxValue)]
        public int Pagina { get; init; }

        [Required]
        [Range(1, 50)]
        public int ElementiPerPagina { get; init; }

        [Required]
        public IReadOnlyList<CriterioRicerca>? CriteriRicerca { get; init; } = null;

        public IReadOnlyList<CriterioOrdinamento>? CriteriOrdinamento { get; init; } = null;
    }

}
