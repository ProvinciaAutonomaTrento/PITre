// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects
{
    public class Allegato : ValueObject
    {
        public Allegato() : base()
        {
        }

        [Required]
        public IdDoc IdDoc { get; init; }

        [Required]
        public TextValue Descrizione { get; init; }

        [Required]
        public TipologieAllegatiEnum TipologiaAllegato { get; init; }

        public IDictionary<string, string>? Metadata { get; init; } = null;
    }
}
