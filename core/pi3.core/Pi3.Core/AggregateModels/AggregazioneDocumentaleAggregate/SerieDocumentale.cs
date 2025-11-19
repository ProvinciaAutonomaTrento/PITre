// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate
{
    public class SerieDocumentale : Entity<string>
    {
        #region Public Members

        internal SerieDocumentale(string id, TextValue? denominazione = null)
        {
            this.Id = id;
            this.Denominazione = denominazione;
        }

        public TextValue Denominazione { get; protected set; }

        #endregion

        #region Private Members

        #endregion
    }

}
