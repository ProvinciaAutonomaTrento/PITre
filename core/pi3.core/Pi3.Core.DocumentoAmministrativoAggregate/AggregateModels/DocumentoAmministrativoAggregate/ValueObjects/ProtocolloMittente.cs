// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
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
