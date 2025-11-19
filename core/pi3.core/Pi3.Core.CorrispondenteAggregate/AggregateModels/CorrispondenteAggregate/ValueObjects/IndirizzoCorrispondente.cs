// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.CorrispondenteAggregate.ValueObjects
{
    public class IndirizzoCorrispondente : ValueObject
    {
        public IndirizzoCorrispondente()
        { }

        public string? Indirizzo { get; init; }
        public string? Provincia { get; init; }
        public string? Cap { get; init; }
        public string? Nazione { get; init; }
        public string? Citta { get; init; }
        public string? Localita { get; init; }
        public string? Fax { get; init; }
        public string? TelefonoPrincipale { get; init; }
        public string? TelefonoSecondario { get; init; }
    }
}
