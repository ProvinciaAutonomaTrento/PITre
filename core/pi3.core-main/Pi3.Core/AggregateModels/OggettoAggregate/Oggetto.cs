// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.AggregateModels.ElementAggregate;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.OggettoAggregate
{
    public class Registro : Entity<string>
    {
        #region Public Members
        internal Registro(string id, string? code = null, string? description = null)
        {
            this.Id = id;
            this.Code = code;
            this.Description = description;
        }

        public string? Code { get; protected set; }
        public string? Description { get; protected set; }

        #endregion

        #region Private Members

        #endregion
    }


    public class OggettoCreatoEvent : ElementCreatedEvent 
    {
        public OggettoCreatoEvent()
        { }

        public string IdRegistro { get; init; }

        public string? CodiceRegistro { get; init; }

        public string? DescrizioneRegistro { get; init; }
    }


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
            : base(id, idTenant, "Oggetto", creationDate, name, description)
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

        public Registro Register { get; protected set; }

        #endregion

        #region Private Members

        protected void Handle(OggettoCreatoEvent @event)
        {
            base.Handle((ElementCreatedEvent)@event);
            this.Register = new Registro(@event.IdRegistro, @event.CodiceRegistro, @event.DescrizioneRegistro);
        }

        #endregion
    }

}
