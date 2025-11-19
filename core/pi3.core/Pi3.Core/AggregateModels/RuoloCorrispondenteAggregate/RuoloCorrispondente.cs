// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.AggregateModels.CorrispondenteAggregate;
using Pi3.Core.AggregateModels.ElementAggregate;
using Pi3.Core.AggregateModels.UOCorrispondenteAggregate;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.RuoloCorrispondenteAggregate
{
    public class RuoloCorrispondente : Corrispondente
    {
        #region Public Members

        public RuoloCorrispondente(string idTenant, DateTime creationDate, TextValue name, TextValue description, string? idRegistro = null)
            : this(null, idTenant, creationDate, name, description, idRegistro)
        {
        }

        public RuoloCorrispondente(
            string id,
            string idTenant,
            DateTime creationDate,
            TextValue name,
            TextValue description,
            string? idRegistro)
        {
            this.ApplyChange(new RuoloCorrispondenteCreatedEvent()
            {
                Id = id,
                IdTenant = idTenant,
                TypeName = "RuoloCorrispondente",
                CreationDate = creationDate,
                Name = name,
                Description = description,
                IdRegistro = idRegistro
            });
        }

        #endregion

        #region Private Members
        public virtual void Handle(RuoloCorrispondenteCreatedEvent @event)
        {         
            base.Handle((ElementCreatedEvent)@event);

            this._email = new List<EmailCorrispondente>();
            this.IdRegistro = @event.IdRegistro;
        }
        #endregion
    }
}
