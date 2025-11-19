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
    public class DatiRegistrazione : ValueObject
    {
        public DatiRegistrazione()
        {
        }

        [Required(AllowEmptyStrings = false)]
        public string IdRegistro { get; init; }

        [Required(AllowEmptyStrings = false)]
        [RegularExpression("[A-Za-z0-9_\\.\\-]{1,16}")]
        public string CodiceRegistro { get; init; }

        [Required(AllowEmptyStrings = false)]
        public string Codice { get; init; }

        [Required()]
        [Range(1, 9999)]
        public int Progressivo { get; init; }
    }
}
