// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate
{
    public enum TipiAssegnazioneEnum
    {
        PerCompetenza,
        PerConoscenza
    }

    public class Assegnazione : ValueObject
    {
        public Assegnazione()
        { }

        [Required(AllowEmptyStrings = false)]
        public TipiAssegnazioneEnum TipoAssegnazione { get; init; }

        [Required]
        public Assegnatario SoggettoAssegnatario { get; init; }

        [Required]
        public DateTime DataInizioAssegnazione { get; init; }

        public DateTime? DataFineAssegnazione { get; init; }
    }
}
