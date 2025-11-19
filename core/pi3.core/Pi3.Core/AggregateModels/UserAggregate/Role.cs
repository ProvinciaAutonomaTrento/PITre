// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.UserAggregate
{
    public class Role : IEntity<string>
    {
        public Role(string id, string? title = null, bool? @default = null)
        {
            this.Id = id;
            this.Title = title;
            this.Default = @default;
        }

        public string Id { get; protected set; }

        public string? Title { get; protected set; }

        public bool? Default { get; protected set; }

        public void SetAsDefault(bool value)
        {
            this.Default = value;
        }
    }
}
