// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects
{
    public class DatiRegistro : ValueObject
    {
        public DatiRegistro()
        { }

        [Required]
        public string IdRegistro { get; init; }

        [RegularExpression("[A-Za-z0-9_\\.\\-]{1,16}")]
        public string? CodiceRegistro { get; init; } = null;

        public TextValue? DescrizioneRegistro { get; init; } = null;
    }
}
