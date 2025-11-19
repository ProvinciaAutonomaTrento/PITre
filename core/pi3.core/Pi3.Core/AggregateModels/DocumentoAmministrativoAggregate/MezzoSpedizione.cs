// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate
{
    public class MezzoSpedizione : Entity<string>
    {
        #region Public Members

        public MezzoSpedizione(string id, TextValue? descrizione = null)
        {
            this.Id = id;
            this.Descrizione = descrizione;
        }

        public TextValue? Descrizione { get; protected set; }

        #endregion

        #region Private Members

        #endregion
    }
}
