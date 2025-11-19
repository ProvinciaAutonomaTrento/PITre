// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.AggregateModels.DelegaAggregate.Entities;
using Pi3.Core.AggregateModels.DelegaAggregate.Events;
using Pi3.Core.AggregateModels.DelegaAggregate.Exceptions;
using Pi3.Core.AggregateModels.DelegaAggregate.ValueObjects;
using Pi3.Core.AggregateModels.ElementAggregate;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.DelegaAggregate
{

    public class Delega : Element
    {
        #region Public Members

        protected Delega() : base()
        { }

        public Delega(string idTenant, DateTime creationDate, TextValue name, TextValue? description = null)
            : base(idTenant, nameof(Delega), creationDate, name, description)
        {
        }

        public Delega(
            string id,
            string idTenant,
            DateTime creationDate,
            TextValue name,
            TextValue? description = null)
            : base(id, idTenant, nameof(Delega), creationDate, name, description)
        {
        }

        public UtenteDelegante? UtenteDelegante { get; protected set; }

        public void AssignUtenteDelegante(string id, string? userId, string? cognome, string? nome)
        {
            AssertStatoDelega();

            this.ApplyChange(new UtenteDeleganteAssignedEvent()
            {
                Id = this.Id,
                IdUtente = id,
                UserId = userId,
                Cognome = cognome,
                Nome = nome
            });
        }

        public GruppoDelegante? GruppoDelegante { get; protected set; }

        public void AssignGruppoDelegante(string id, string? codice, string? descrizione)
        {
            AssertStatoDelega();

            this.ApplyChange(new GruppoDeleganteAssignedEvent()
            {
                Id = this.Id,
                IdGruppo = id,
                Codice = codice,
                Descrizione = descrizione
            });
        }

        public UtenteDelegato UtenteDelegato { get; protected set; }

        public void AssignUtenteDelegato(string id, string? userId, string? cognome, string? nome)
        {
            AssertStatoDelega();

            this.ApplyChange(new UtenteDelegatoAssignedEvent()
            {
                Id = this.Id,
                IdUtente = id,
                UserId = userId,
                Cognome = cognome,
                Nome = nome
            });
        }

        public GruppoDelegato GruppoDelegato { get; protected set; }

        public void AssignGruppoDelegato(string id, string? codice, string? descrizione)
        {
            AssertStatoDelega();

            this.ApplyChange(new GruppoDelegatoAssignedEvent()
            {
                Id = this.Id,
                IdGruppo = id,
                Codice = codice,
                Descrizione = descrizione
            });
        }

        public DateTime DataDecorrenza { get; protected set; }

        public void ChangeDataDecorrenza(DateTime newDate)
        {
            AssertStatoDelega();

            this.ApplyChange(new DataDecorrenzaChangeEvent()
            {
                Id = this.Id,
                NewData = newDate
            });
        }

        public DateTime? DataScadenza { get; protected set; }

        public void ChangeDataScadenza(DateTime? newDate)
        {
            AssertStatoDelega();

            this.ApplyChange(new DataScadenzaChangeEvent()
            {
                Id = this.Id,
                NewData = newDate
            });
        }

        public bool InEsercizio { get; protected set; } = false;

        public void Esercita()
        {
            this.ApplyChange(new EsercitataEvent()
            {
                Id = this.Id,
                InEsercizio = true
            });
        }

        public void Dismetti()
        {
            this.ApplyChange(new DismessaEvent()
            {
                Id = this.Id,
                InEsercizio = false
            });
        }

        public StatoDelegaEnum Stato { get; protected set; } = StatoDelegaEnum.Impostata;

        public void Revoca()
        {
            this.ApplyChange(new RevocataEvent()
            {
                Id = this.Id
            });
        }

        public override bool IsValid()
        {
            return !GetErrors().Any();
        }

        public virtual IEnumerable<Pi3Exception> GetErrors()
        {
            List<Pi3Exception> pi3Exceptions = new List<Pi3Exception>();

            if (this.UtenteDelegante == null || string.IsNullOrEmpty(this.UtenteDelegante.Id))
                pi3Exceptions.Add(new UtenteDeleganteNotValidPi3Exception());

            if (this.UtenteDelegato == null || string.IsNullOrEmpty(this.UtenteDelegato.Id))
                pi3Exceptions.Add(new UtenteDelegatoNotValidPi3Exception());

            if (this.GruppoDelegante == null || string.IsNullOrEmpty(this.GruppoDelegante.Id))
                pi3Exceptions.Add(new GruppoDeleganteNotValidPi3Exception());

            if (this.GruppoDelegato == null || string.IsNullOrEmpty(this.GruppoDelegato.Id))
                pi3Exceptions.Add(new GruppoDelegatoNotValidPi3Exception());

            return pi3Exceptions;
        }

        #endregion

        #region Private Members

        protected void AssertStatoDelega()
        {
            if(this.Stato == StatoDelegaEnum.Attiva)
            {
                if(this.InEsercizio)
                    throw new ModificaDelegaAttivaPi3Exception();
            }
        }

        protected void AssignStatoDelega()
        {
            this.Stato = StatoDelegaEnum.Impostata;

            if (this.DataDecorrenza < DateTime.Now && (this.DataScadenza == new DateTime() || this.DataScadenza > DateTime.Now))
                this.Stato = StatoDelegaEnum.Attiva;

            if (this.DataScadenza != new DateTime() && this.DataScadenza < DateTime.Now)
                this.Stato = StatoDelegaEnum.Scaduta;
        }

        protected virtual void Handle(UtenteDeleganteAssignedEvent @event)
        {
            this.UtenteDelegante = new UtenteDelegante(@event.IdUtente, @event.UserId, @event.Cognome, @event.Nome);
        }

        protected virtual void Handle(UtenteDelegatoAssignedEvent @event)
        {
            this.UtenteDelegato = new UtenteDelegato(@event.IdUtente, @event.UserId, @event.Cognome, @event.Nome);
        }

        protected virtual void Handle(GruppoDeleganteAssignedEvent @event)
        {
            this.GruppoDelegante = new GruppoDelegante(@event.IdGruppo, @event.Codice, @event.Descrizione);
        }

        protected virtual void Handle(GruppoDelegatoAssignedEvent @event)
        {
            this.GruppoDelegato = new GruppoDelegato(@event.IdGruppo, @event.Codice, @event.Descrizione);
        }

        protected virtual void Handle(RevocataEvent @event)
        {
            this.DataScadenza = DateTime.Now;
        }

        protected virtual void Handle(DismessaEvent @event)
        {
            this.InEsercizio = @event.InEsercizio;
        }

        protected virtual void Handle(EsercitataEvent @event)
        {
            this.InEsercizio = @event.InEsercizio;
        }

        protected virtual void Handle(DataDecorrenzaChangeEvent @event)
        {
            this.DataDecorrenza = @event.NewData;

            AssignStatoDelega();
        }

        protected virtual void Handle(DataScadenzaChangeEvent @event)
        {
            this.DataScadenza = @event.NewData;

            AssignStatoDelega();
        }
        #endregion
    }
}
