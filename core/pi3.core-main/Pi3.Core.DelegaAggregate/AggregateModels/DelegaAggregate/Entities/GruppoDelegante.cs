// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.DelegaAggregate.Entities
{
    public class GruppoDelegante : Entity<string>
    {
        #region Public Members
        internal GruppoDelegante(string id, string? codice, string? descrizione)
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
