// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.AggregateModels.TrasmissioneAggregate.ValueObjects;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.TrasmissioneAggregate.Entities
{
    public class RagioneTrasmissione : Entity<string>
    {
        #region Public Members

        internal RagioneTrasmissione(string id, string? nome = null, bool? conWorkflow = false, CessioneDirittiRagione? cessioneDirittiRagione = null)
        {
            this.Id = id;
            this.Nome = nome;
            this.ConWorkflow = conWorkflow;
            this.CessioneDirittiRagione = cessioneDirittiRagione;
        }

        public string? Nome { get; protected set; } = null;

        public bool? ConWorkflow { get; protected set; } = false;

        public CessioneDirittiRagione? CessioneDirittiRagione { get; protected set; } = null;

        #endregion

        #region Private Members

        #endregion
    }
}
