// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.SeedWork
{
    public abstract class AggregateRoot<K> : Entity<K>, IAggregateRoot<K>
    {
        protected List<dynamic> _uncommittedChanges;
        //protected List<dynamic> _committedChanges;

        public K Id { get; protected set; }

        public int Version { get; protected set; }

        public int EventVersion { get; protected set; }

        protected AggregateRoot()
        {
            this._uncommittedChanges = new List<dynamic>();
            //this._committedChanges = new List<dynamic>();
        }

        public virtual IEnumerable<dynamic> GetUncommittedChanges()
        {
            return this._uncommittedChanges;
        }

        //public IEnumerable<dynamic> GetCommittedChanges()
        //{
        //    return this._committedChanges;
        //}

        public virtual void MarkChangesAsCommitted()
        {
            //this._committedChanges.AddRange(this._uncommittedChanges);

            this._uncommittedChanges.Clear();
        }

        public virtual void MarkChangesAsCommitted(Type ofType)
        {
            //this._committedChanges.AddRange(this._uncommittedChanges.Where(c => c.GetType() == ofType));

            this._uncommittedChanges.RemoveAll(c => c.GetType() == ofType);
        }

        internal virtual void LoadFromHistory(params IEvent[] history)
        {
            foreach (var e in history)
                this.ApplyChange(e, false);

            this.Version = history.Last().Version;
            this.EventVersion = Version;
        }

        public virtual bool IsValid()
        {
            return !this.GetErrors().Any();
        }

        public virtual IEnumerable<Pi3Exception> GetErrors()
        {
            return new Pi3Exception[0];
        }

        protected virtual void ApplyChange(IEvent @event)
        {
            this.ApplyChange(@event, true);
        }

        protected virtual void ApplyChange(IEvent @event, bool isNew)
        {
            @event.SetOwnerAggregate(this);

            var handleMethod = this.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .FirstOrDefault(m => m.Name == "Handle" && m.GetParameters().Count(p => p.ParameterType == @event.GetType()) > 0);

            if (handleMethod == null)
                throw new MethodNotImplementedPi3Exception("Handle");

            handleMethod.Invoke(this, new object[] { @event });

            if (isNew)
            {
                _uncommittedChanges.Add(@event);
            }
        }
    }
}
