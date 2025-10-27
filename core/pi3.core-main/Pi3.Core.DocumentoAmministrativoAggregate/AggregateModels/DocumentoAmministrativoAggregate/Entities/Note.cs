// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Entities
{
    public class Nota : Entity<string>
    {
        internal Nota(string id, TipologieVisibilitaNotaEnum tipologiaVisibilita, TextValue testo)
        {
            Id = id;
            TipologiaVisibilita = tipologiaVisibilita;
            Testo = testo;
        }

        public TipologieVisibilitaNotaEnum TipologiaVisibilita { get; protected set; }

        public TextValue Testo { get; protected set; }
    }
}
