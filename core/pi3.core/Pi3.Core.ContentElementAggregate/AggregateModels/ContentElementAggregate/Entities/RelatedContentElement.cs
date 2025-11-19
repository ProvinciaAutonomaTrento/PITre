// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pi3.Core.SeedWork;

namespace Pi3.Core.AggregateModels.ContentElementAggregate.Entities
{
    public class RelatedContentElement : Entity<string>
    {
        internal RelatedContentElement(string id, bool asParent)
        {
            Id = id;
            AsParent = asParent;
        }

        public string Id { get; protected set; }

        public bool AsParent { get; protected set; }
    }
}
