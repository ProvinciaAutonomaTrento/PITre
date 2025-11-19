// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.AggregateModels.ElementAggregate;
using Pi3.Core.AggregateModels.ElementAggregate.Events;
using Pi3.Core.AggregateModels.OggettoAggregate.Entities;
using Pi3.Core.AggregateModels.OggettoAggregate.Events;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.OggettoAggregate
{
    public class Oggetto : Element
    {
        #region Public Members

        public Oggetto(string idTenant, DateTime creationDate, TextValue name, string registerId,
            string? registerCode = null,
            string? registerDescription = null, TextValue? description = null)
            : this(null, idTenant, creationDate, name, registerId, registerCode, registerDescription, description)
        {
        }

        public Oggetto(
            string id,
            string idTenant,
            DateTime creationDate,
            TextValue name,
            string registerId,
            string? registerCode = null,
            string? registerDescription = null,
            TextValue? description = null)
            : base(id, idTenant, nameof(Oggetto), creationDate, name, description)
        {
            this.ApplyChange(new OggettoCreatoEvent
            {
                Id = id,
                IdTenant = idTenant,
                CreationDate = creationDate,
                Name = name,
                Description = description,
                IdRegistro = registerId,
                CodiceRegistro = registerCode,
                DescrizioneRegistro = registerDescription
            });
        }

        public Registro Registro { get; protected set; }

        #endregion

        #region Private Members

        protected Oggetto() : base()
        { }

        protected void Handle(OggettoCreatoEvent @event)
        {
            base.Handle((ElementCreatedEvent)@event);

            this.Registro = new Registro(@event.IdRegistro, @event.CodiceRegistro, @event.DescrizioneRegistro);
        }

        #endregion
    }

}
