// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects
{
    public class ProtocolloMittente : ValueObject
    {
        public ProtocolloMittente() : base()
        {
        }
        public string? Segnatura { get; init; }
        public DateTime? Data { get; init; }
        public DateTime? DataArrivo { get; init; }
    }
}
