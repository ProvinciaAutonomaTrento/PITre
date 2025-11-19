// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.SeedWork
{
    public interface IRepository<A, in K> where A : IAggregateRoot<K>
    {
        Task<bool> Exists(K idAggregate);
        Task<A> Get(K idAggregate, params ILoadBehavior[] loadBehaviors);
        Task Add(A aggregate);
        Task Update(A aggregate);
        Task Delete(A aggregate);
        Task Load(A aggregate, params ILoadBehavior[] loadBehaviors);
    }
}
