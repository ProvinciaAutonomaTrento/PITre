// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Microsoft.Extensions.Logging;
using Pi3.Core.AggregateModels.ElementAggregate;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.ListaDistribuzioneAggregate
{
    
    public class ListaDistribuzione : Element
    {
        protected ListaDistribuzione() : base()
        {
        }

        public ListaDistribuzione(string idTenant, DateTime creationDate, TextValue name, TextValue description)
            : base(idTenant, "ListaDistribuzione", creationDate, name, description)
        {
            description = description ?? throw new ArgumentNullException(nameof(description));
        }

        public ListaDistribuzione(string id, string idTenant, DateTime creationDate, TextValue name, TextValue description = null)
            : base(id, idTenant, "ListaDistribuzione", creationDate, name, description)
        {
            description = description ?? throw new ArgumentNullException(nameof(description));
        }

        public Autore Autore { get; protected set; }

        public void AssignAutorePersona(string id, string? userId = null, string? nome = null, string? cognome = null)
        {
            this.ApplyChange(new AutorePersonaAssignedEvent()
            {
                Id = this.Id,
                IdUtente = id,
                UserId = userId,
                Nome = nome,
                Cognome = cognome
            });
        }

        public void AssignAutoreRuolo(string id, string? codiceGruppo = null, TextValue? descrizione = null)
        {
            this.ApplyChange(new AutoreRuoloAssignedEvent()
            {
                Id = this.Id,
                CodiceGruppo = codiceGruppo,
                IdGruppo = id,                
                Descrizione = descrizione
            });
        }

        protected List<Destinatario> _Destinatari = new List<Destinatario>();

        public IReadOnlyList<Destinatario> Destinatari { get { return this._Destinatari.AsReadOnly(); }}

        public void AddDestinatario(string id, TipiDestinatariEnum tipoDestinatario, TextValue? descrizione, bool? esterno)
        {            
            if (_Destinatari.Count==0 || !_Destinatari.Where(w => w.Id == id).Any())
            {
                this.ApplyChange(new DestinatarioAddedEvent()
                {
                    Id = this.Id,
                    IdDestinatario = id,
                    TipoDestinatario = tipoDestinatario,
                    Descrizione = descrizione,
                    Esterno = esterno
                });
            }
            else
            {
                throw new DestinatarioAlreadyExistPi3Exception(id);
                
            }
        }

        public void RemoveDestinatario(string id)
        {
            if (_Destinatari.Where(w => w.Id == id).Any())
            {
                this.ApplyChange(new DestinatarioRemovedEvent()
                {
                    Id = this.Id,
                    IdDestinatario = id
                });
            }
            else
            {
                throw new DestinatarioAlreadyDeletedPi3Exception(id);
            }
        }

        protected override void Handle(ElementCreatedEvent @event)
        {
            base.Handle(@event);
            this._Destinatari = new List<Destinatario>();
        }

        protected virtual void Handle(AutorePersonaAssignedEvent @event)
        {
            Autore = new AutorePersona(@event.Id, @event.UserId, @event.Cognome, @event.Nome);
        }

        protected virtual void Handle(AutoreRuoloAssignedEvent @event)
        {
            Autore = new AutoreGruppo(@event.Id, @event.IdGruppo, @event.Descrizione);
        }

        protected virtual void Handle(ListaDistribuzioneAggregate.DestinatarioAddedEvent @event)
        {
            _Destinatari.Add(new Destinatario(@event.IdDestinatario, @event.TipoDestinatario, @event.Descrizione, @event.Esterno));

        }
        
        protected virtual void Handle(DestinatarioRemovedEvent @event)
        {
            _Destinatari.RemoveAll(r => r.Id == @event.IdDestinatario);
        }
    }


}




