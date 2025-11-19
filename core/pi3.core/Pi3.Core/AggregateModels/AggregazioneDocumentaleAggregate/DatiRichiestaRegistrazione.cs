// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate
{
    public class DatiRichiestaRegistrazione : ValueObject
    {
        public DatiRichiestaRegistrazione()
        {
        }

        [Required(AllowEmptyStrings = false)]
        public string? IdRegistro { get; init; } = null;
    }
}
