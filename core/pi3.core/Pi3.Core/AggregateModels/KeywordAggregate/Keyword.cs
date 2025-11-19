// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.AggregateModels.ElementAggregate;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.KeywordAggregate
{
    public class Keyword : Element
    {


        protected Keyword() : base()
        { }

        public Keyword(string idTenant, DateTime creationDate, TextValue name, TextValue? description = null)
            : base(idTenant, "Keyword", creationDate, name, description)
        {
        }

        public Keyword(
            string id,
            string idTenant,
            DateTime creationDate,
            TextValue name,
            TextValue? description = null)
            : base(id, idTenant, "Keyword", creationDate, name, description)
        {
        }

    }  

    
}
