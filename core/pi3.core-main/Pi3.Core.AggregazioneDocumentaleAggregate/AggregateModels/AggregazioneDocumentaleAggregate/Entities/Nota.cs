// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.Entities
{
    public class Nota : Entity<string>
    {
        internal Nota(string id, TipologiaVisibilitaNotaEnum tipologiaVisibilita, TextValue testo)
        {
            Id = id;
            TipologiaVisibilita = tipologiaVisibilita;
            Testo = testo;
        }

        public TipologiaVisibilitaNotaEnum TipologiaVisibilita { get; protected set; }

        public TextValue Testo { get; protected set; }
    }
}
