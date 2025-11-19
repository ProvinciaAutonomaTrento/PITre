// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate
{
    public enum TipologiaVisibilitaNotaEnum
    {
        Personale,
        Ruolo,
        RF,
        Pubblica
    }

    public class Nota : Entity<string>
    {
        internal Nota(string id, TipologiaVisibilitaNotaEnum tipologiaVisibilita, TextValue testo)
        {
            this.Id = id;
            this.TipologiaVisibilita = tipologiaVisibilita;
            this.Testo = testo;
        }

        public TipologiaVisibilitaNotaEnum TipologiaVisibilita { get; protected set; }
        
        public TextValue Testo { get; protected set; }
    }
}
