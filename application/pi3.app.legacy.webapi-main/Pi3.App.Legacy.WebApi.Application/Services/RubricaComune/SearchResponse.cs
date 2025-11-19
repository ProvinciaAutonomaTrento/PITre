// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Services.RubricaComune
{
    public class SearchResponse : ValueObject
    {
        public int TotalePagine { get; init; }

        public int TotaleCorrispondenti { get; init; }

        public IReadOnlyList<Corrispondente>? Corrispondenti { get; init; } = null;

    }
}
