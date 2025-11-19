// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.TrasmissioneAggregate
{
    public class GruppoDestinatario : Entity<string>
    {
        #region Public Members

        internal GruppoDestinatario(string id, 
            string? codice = null, 
            TextValue? descrizione = null)
        {
            this.Id = id;
            this.Codice = codice;
            this.Descrizione = descrizione;
        }

        public string? Codice { get; protected set; } = null;

        public TextValue? Descrizione { get; protected set; } = null;

        #endregion

        #region Private Members

        #endregion
    }
}
