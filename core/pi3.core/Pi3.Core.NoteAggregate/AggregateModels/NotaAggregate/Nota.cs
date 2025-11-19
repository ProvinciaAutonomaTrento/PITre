// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.EntityFrameworkCore.Metadata;
using Pi3.Core.AggregateModels.ElementAggregate;
using Pi3.Core.AggregateModels.ElementAggregate.Events;
using Pi3.Core.AggregateModels.NotaAggregate.Entities;
using Pi3.Core.AggregateModels.NotaAggregate.Events;
using Pi3.Core.AggregateModels.NotaAggregate.Resources;
using Pi3.Core.AggregateModels.NotaAggregate.ValueObjects;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.NotaAggregate
{
    public class Nota : Element
    {
        #region Public Members

        public Nota(string id, string idTenant, DateTime creationDate,TextValue nome ,TextValue? description, AutoreNota autore, string idOggetto, TipiOggettoEnum tipoOggetto, TipoAccessoNotaEnum tipoAccesso, string? idAccessoRF) 
        {
            this.ApplyChange(new NotaCreataEvent()
            {
                Id = id,
                IdTenant = idTenant,
                TypeName = nameof(Nota),
                CreationDate = creationDate,
                Name = nome,
                Description = description,
                Autore = autore,
                IdOggettoAssociato = idOggetto,
                TipoOggettoAssociato= tipoOggetto,
                TipoAccesso = tipoAccesso,
                IdIdAccessoRF = idAccessoRF
            }); 
        }
        

        public Nota(string idTenant, DateTime creationDate, TextValue name,TextValue? description, AutoreNota autore, string idOggetto, TipiOggettoEnum tipoOggetto, TipoAccessoNotaEnum tipoAccesso, string? idAccessoRF) 
            : this(null,  idTenant,  creationDate,  name, description,  autore, idOggetto, tipoOggetto,  tipoAccesso, idAccessoRF)
        {           
        }

        public TipoAccessoNotaEnum TipoAccesso { get; protected set; }

        public string? IdAccessoRF { get; protected set; }

        public AutoreNota Autore { get; protected set; }

        public OggettoAssociato OggettoAssociato { get; protected set; }    

        public void SetAccessoRF(string idAccessoRF)
        {
            if(TipoAccesso > TipoAccessoNotaEnum.RF)          
                throw new NotSupportedPi3Exception(ErrorDescriptions.CambioVisibilitaNonConsentito, ErrorDescriptions.ResourceManager);

            this.ApplyChange(new TipoAccessoChangedEvent() { Id=this.Id, IdAccessoRF = idAccessoRF, TipoAccesso = TipoAccessoNotaEnum.RF });
        }

        public void SetAccessoRuolo()
        {
            if (TipoAccesso > TipoAccessoNotaEnum.Ruolo)
                throw new NotSupportedPi3Exception(ErrorDescriptions.CambioVisibilitaNonConsentito, ErrorDescriptions.ResourceManager);

            this.ApplyChange(new TipoAccessoChangedEvent() { Id = this.Id, IdAccessoRF = null, TipoAccesso = TipoAccessoNotaEnum.Ruolo });
        }

        public void SetAccessoPersonale()
        {
            if (TipoAccesso > TipoAccessoNotaEnum.Personale)
                throw new NotSupportedPi3Exception(ErrorDescriptions.CambioVisibilitaNonConsentito, ErrorDescriptions.ResourceManager);

            this.ApplyChange(new TipoAccessoChangedEvent() { Id = this.Id, IdAccessoRF = null, TipoAccesso = TipoAccessoNotaEnum.Personale });
        }

        public void SetAccessoPubblico()
        {
            this.ApplyChange(new TipoAccessoChangedEvent() { Id = this.Id, IdAccessoRF = null, TipoAccesso = TipoAccessoNotaEnum.Pubblica });
        }

        #endregion

        #region Private Members

        protected Nota() : base()
        { }

        protected void Handle(TipoAccessoChangedEvent @event)
        {
            this.TipoAccesso = @event.TipoAccesso;
            this.IdAccessoRF = @event.IdAccessoRF;
        }    

        protected void Handle(NotaCreataEvent @event)
        {
            base.Handle((ElementCreatedEvent) @event);

            this.Autore = @event.Autore;

            this.OggettoAssociato = new OggettoAssociato(@event.IdOggettoAssociato, @event.TipoOggettoAssociato);
            this.TipoAccesso = @event.TipoAccesso;
            this.IdAccessoRF = @event.IdIdAccessoRF;    
        }

        #endregion
    }
}
