// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.AggregateModels.ElementAggregate;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.RoleTypeAggregate
{
    public class RoleType : Element
    {
        #region Public Members

        protected RoleType() : base()
        {
        }

        public RoleType(string idTenant, DateTime creationDate, TextValue name, TextValue? description = null)
                : base(null, idTenant, "User", creationDate, name, description)
        {
        }

        public RoleType(string id, string idTenant, DateTime creationDate, TextValue name, TextValue? description = null)
            : base(id, idTenant, "User", creationDate, name, description)
        {
        }

        public void ChangeHierarchicalLevel(int newHierarchicalLevel)
        {
            this.ApplyChange(new RoleTypeHierarchicalLevelChangedEvent()
            {
                Id = this.Id, 
                NewHierarchicalLevel = newHierarchicalLevel
            });
        }

        public int? HierarchicalLevel 
        { 
            get; 
            protected set; 
        }

        #endregion

        #region Private Members

        protected void Handle(RoleTypeHierarchicalLevelChangedEvent @event)
        {
            this.HierarchicalLevel = @event.NewHierarchicalLevel;
        }

        #endregion
    }
}
