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
    public class FormatoFile : ValueObject
    {
        public FormatoFile() : base()
        {
        }

        public string Nome { get; init; }

        public string Estensione { get; init; }

        public string ContentType { get; init; }

        public string Produttore { get; init; }

        public string Versione { get; init; }
    }
}
