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
    public class DatiStampa : ValueObject
    {
        public DatiStampa()
                    : base()
        {

        }

        public DatiRegistrazione PrimoElementoStampato { get; set; }

        public DatiRegistrazione UltimoElementoStampato { get; set; }

        public long? AnnoStampa { get; set; }

        public string? CodiceRegistro { get; set; }

        public TipologieContatoriRepertorioEnum? TipoContatore { get; set; }

        [Required]
        public TipologieStampaEnum TipoStampa { get; set; }

    }
}
