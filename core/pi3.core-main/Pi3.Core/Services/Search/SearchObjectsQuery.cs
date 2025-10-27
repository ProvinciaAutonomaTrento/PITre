// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Services.Search
{
    public class SearchObjectsQuery : ValueObject
    {
        [Required]
        public int? Take { get; init; } = null;

        public int? Skip { get; init; } = null;

        public string? Query { get; init; } = null;
    }
}
