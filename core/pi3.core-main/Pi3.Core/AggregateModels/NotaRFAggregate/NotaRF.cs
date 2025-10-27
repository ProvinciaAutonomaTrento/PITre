// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Microsoft.EntityFrameworkCore.Metadata;
using Pi3.Core.AggregateModels.ElementAggregate;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.NotaRFAggregate
{
    public class NotaRF: Element
    {

        public NotaRF(string id, string idTenant, DateTime creationDate, TextValue nome, TextValue? description, string IdRF, string? CodiceRF, TextValue? DescrizioneRF)
        {
            this.ApplyChange(new NotaRFCreatedEvent()
            {
                Id = id,
                IdTenant = idTenant,
                TypeName = "NotaRF",
                CreationDate = creationDate,
                Name = nome,
                Description = description,
                IdRF = IdRF,
                CodiceRF = CodiceRF,
                DescrizioneRF = DescrizioneRF
            });
        }
      
        public NotaRF(string idTenant, DateTime creationDate, TextValue nome, TextValue? description, string IdRF, string? CodiceRF, TextValue? DescrizioneRF/*, RF rf*/)
            : this(null, idTenant, creationDate, nome, description, IdRF, CodiceRF, DescrizioneRF)
        {
        }

        public void ChangeRFNota(string idRF, string? codiceRF, TextValue? descrizioneRF)
        {
            idRF = idRF ?? throw new ArgumentNullException(nameof(idRF));
            this.ApplyChange(new RFChangedEvent() { Id = this.Id, NewIdRF = idRF, NewCodiceRF = codiceRF, NewDescrizioneRF = descrizioneRF });
        }


        public RF RF { get; protected set; }

        //handler er la modifica dell'RF associato alla nota
        protected void Handle(RFChangedEvent @event)
        {        
            this.RF = new RF(@event.NewIdRF, @event.NewCodiceRF, @event.NewDescrizioneRF);            
        }

        //l'handler che si occupa effettivamente della creazione
        protected void Handle(NotaRFCreatedEvent @event)
        {
            base.Handle((ElementCreatedEvent)@event);
            this.RF = new RF(@event.IdRF, @event.CodiceRF, @event.DescrizioneRF);                   
        }
    }


    public class RF : Entity<string>
    {
        #region Public Members

        internal RF(string id, string? codice = null, TextValue? descrizione = null )
        {
            this.Id = id;
            this.Codice = codice;
            this.Descrizione = descrizione;
        }

        public string? Codice { get; protected set; }

        public TextValue? Descrizione { get; protected set; }

        #endregion

        #region Private Members

        #endregion
    }

}
