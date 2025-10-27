// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.NotaRFAggregate.Entities
{
    public class RF : Entity<string>
    {
        #region Public Members

        internal RF(string id, string? codice = null, TextValue? descrizione = null)
        {
            this.Id = id;
            this.Codice = codice;
            this.Descrizione = descrizione;
        }

        public string? Codice { get; protected set; }

        public TextValue? Descrizione { get; protected set; }

        #endregion

        #region Private Members

        #endregion
    }
}
