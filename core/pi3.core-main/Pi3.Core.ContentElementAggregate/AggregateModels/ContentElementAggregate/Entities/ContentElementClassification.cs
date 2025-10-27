// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pi3.Core.SeedWork;
using Microsoft.Extensions.Logging;

namespace Pi3.Core.AggregateModels.ContentElementAggregate.Entities
{
    public class ContentElementClassification : Entity<string>
    {
        #region Public Members

        internal ContentElementClassification(string id, TextValue? name = null, string? code = null)
        {
            Id = id;
            Name = name;
            Code = code;
        }

        public string Id { get; protected set; }

        public TextValue? Name { get; protected set; } = null;

        public string? Code { get; protected set; } = null;

        #endregion

        #region Private Members

        #endregion
    }
}
