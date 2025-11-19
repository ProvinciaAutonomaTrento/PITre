// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.RubricaComune.Core.AggregateModels.CorrispondenteAggregate.ValueObjects
{
    public class Indirizzo : ValueObject
    {
        public Indirizzo()
        { }

        public string? Recapito { get; init; } = null;

        public string? Telefono { get; init; } = null;

        public string? Fax { get; init; } = null;

        public string? Citta { get; init; } = null;

        public string? CAP { get; init; } = null;

        public string? Provincia { get; init; } = null;

        public string? Nazione { get; init; } = null;
    }
}
