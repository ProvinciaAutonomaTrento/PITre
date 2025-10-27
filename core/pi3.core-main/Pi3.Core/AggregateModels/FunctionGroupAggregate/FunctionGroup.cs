// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.AggregateModels.ElementAggregate;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.FunctionGroupAggregate
{
    public class Function : Entity<string>
    {
        #region Public Members

        public Function(string id, string code, TextValue description)
        {
            this.Id = id;
            this.Code = code;
            this.Description = description;
        }

        public string Code { get; protected set; }

        public TextValue Description { get; protected set; }

        #endregion

        #region Private Members

        #endregion
    }

    public class FunctionAdded : Event
    {
        public FunctionAdded()
        { }

        public string IdFunctionGroup { get; init; }

        public Function Function { get; init; }
    }

    public class FunctionRemoved : Event
    {
        public FunctionRemoved()
        { }

        public string IdFunctionGroup { get; init; }

        public Function Function { get; init; }
    }

    public class FunctionGroup : Element
    {
        #region Public Members

        protected FunctionGroup() : base()
        { }

        public FunctionGroup(string idTenant, DateTime creationDate, TextValue name, TextValue? description = null)
            : base(idTenant, "FunctionGroup", creationDate, name, description)
        {
        }

        public FunctionGroup(
            string id,
            string idTenant,
            DateTime creationDate,
            TextValue name,
            TextValue? description = null)
            : base(id, idTenant, "FunctionGroup", creationDate, name, description)
        {
        }

        public void AddFunction(Function function)
        {
            function = function ?? throw new ArgumentNullException(nameof(function));

            this.ApplyChange(new FunctionAdded() { IdFunctionGroup = this.Id, Function = function });
        }

        public void RemoveFunction(Function function)
        {
            function = function ?? throw new ArgumentNullException(nameof(function));

            this.ApplyChange(new FunctionRemoved() { IdFunctionGroup = this.Id, Function = function });
        }

        public IReadOnlyList<Function> Functions
        {
            get
            {
                return this._functions.AsReadOnly();
            }
        }

        #endregion

        #region Private Members

        protected List<Function> _functions;

        protected override void Handle(ElementCreatedEvent @event)
        {
            base.Handle(@event);

            this._functions = new List<Function>();
        }

        protected virtual void Handle(FunctionAdded @event)
        {
            this._functions.Add(@event.Function);
        }

        protected virtual void Handle(FunctionRemoved @event)
        {
            this._functions.Remove(@event.Function);
        }

        #endregion
    }

    public interface IFunctionGroupRepository : IElementRepository<FunctionGroup>
    {
    }

}
