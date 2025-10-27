// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.AggregateModels.ElementAggregate;
using Pi3.Core.AggregateModels.NoteAggregate;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.MezzoSpedizioneAggregate
{
    public class MezzoSpedizione : Element
    {
        #region Public Members

        protected MezzoSpedizione() : base()
        { }

        public MezzoSpedizione(string idTenant, DateTime creationDate, TextValue name, TextValue? description = null)
            : base(idTenant, "MezzoSpedizione", creationDate, name, description)
        {
        }

        public MezzoSpedizione(
            string id,
            string idTenant,
            DateTime creationDate,
            TextValue name,
            TextValue? description = null)
            : base(id, idTenant, "MezzoSpedizione", creationDate, name, description)
        {
        }

        #endregion

        #region Private Members

        #endregion
    }
}
