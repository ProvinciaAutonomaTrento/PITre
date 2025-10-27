// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects
{


    public class IdDoc : ValueObject
    {
        public IdDoc() : base()
        {
        }

        public ImprontaCrittograficaDocumento ImprontaCrittograficaDelDocumento { get; init; }

        [Required]
        public string Identiticativo { get; init; }

        public string? Segnatura { get; init; } = null;
    }
}
