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
    public class Registro : Entity<string>
    {
        #region Public Members
        internal Registro(string id, string? code = null, string? description = null)
        {
            this.Id = id;
            this.Code = code;
            this.Description = description;
        }

        public string? Code { get; protected set; }
        public string? Description { get; protected set; }

        #endregion

        #region Private Members

        #endregion
    }
}
