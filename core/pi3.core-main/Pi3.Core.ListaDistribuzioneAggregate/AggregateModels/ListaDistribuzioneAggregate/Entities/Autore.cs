// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.ListaDistribuzioneAggregate.Entities
{
    public abstract class Autore : Entity<string>
    {
        internal Autore(string id)
        {
            Id = id;
        }

        public string Id { get; protected set; }
    }
}
