// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.SeedWork
{
    public interface IAggregateRoot<K> : IEntity<K>
    {
        public K Id { get; }

        public int Version { get; }

        IEnumerable<dynamic> GetUncommittedChanges();

        //IEnumerable<dynamic> GetCommittedChanges();

        void MarkChangesAsCommitted();

        void MarkChangesAsCommitted(Type ofType);

        bool IsValid();

        IEnumerable<Pi3Exception> GetErrors();
    }
}
