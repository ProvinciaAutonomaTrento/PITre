// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.DelegaAggregate.Entities
{
    public class GruppoDelegato : Entity<string>
    {
        #region Public Members
        internal GruppoDelegato(string id, string? codice, string? descrizione)
        {
            Id = id;
            Codice = codice;
            Descrizione = descrizione;
        }

        public string? Codice { get; protected set; }

        public string? Descrizione { get; protected set; }

        #endregion

        #region Private Members

        #endregion
    }
}
