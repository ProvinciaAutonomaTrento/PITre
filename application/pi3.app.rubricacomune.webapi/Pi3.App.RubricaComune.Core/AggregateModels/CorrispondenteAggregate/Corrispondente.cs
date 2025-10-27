// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Pi3.App.RubricaComune.Core.AggregateModels.CorrispondenteAggregate.Entities;
using Pi3.App.RubricaComune.Core.AggregateModels.CorrispondenteAggregate.Events;
using Pi3.App.RubricaComune.Core.AggregateModels.CorrispondenteAggregate.Exceptions;
using Pi3.App.RubricaComune.Core.AggregateModels.CorrispondenteAggregate.Resources;
using Pi3.App.RubricaComune.Core.AggregateModels.CorrispondenteAggregate.ValueObjects;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.RubricaComune.Core.AggregateModels.CorrispondenteAggregate
{
    public class Corrispondente : AggregateRoot<string>
    {
        #region Public Members

        protected Corrispondente() : base()
        { }

        public Corrispondente(string? id, string codice, string denominazione, TipiEnum tipo, DateTime? dataCreazione, DateTime? dataUltimaModifica)
        {
            codice = codice ?? throw new ArgumentNullException(nameof(codice));
            denominazione = denominazione ?? throw new ArgumentNullException(nameof(denominazione));

            this.ApplyChange(new CorrispondenteCreatoEvent()
            {
                Id = id ?? String.Empty,
                Codice = codice,
                Denominazione = denominazione,
                Tipo = tipo,
                DataCreazione = dataCreazione,
                DataUltimaModifica = dataUltimaModifica
            });
        }

        public Corrispondente(string codice, string denominazione, TipiEnum tipo, DateTime? dataCreazione = null, DateTime? dataUltimaModifica = null)
            : this(null, codice, denominazione, tipo, dataCreazione, dataUltimaModifica)
        {
        }

        public Corrispondente(DatiPubblicazione datiPubblicazione)
        {
            datiPubblicazione = datiPubblicazione ?? throw new ArgumentNullException(nameof(datiPubblicazione));

            this.ApplyChange(new CorrispondentePubblicatoEvent()
            {
                DatiPubblicazione = datiPubblicazione
            });
        }

        public string Codice { get; protected set; } = null!;

        public string Denominazione { get; protected set; } = null!;

        public TipiEnum Tipo { get; protected set; }

        public DateTime DataCreazione { get; protected set; }

        public DateTime? DataUltimaModifica { get; protected set; }

        public Indirizzo? Indirizzo { get; protected set; }

        public string? CodiceFiscale { get; protected set; }

        public string? PartitaIva { get; protected set; }

        public string? UrlApiInteroperabilita { get; protected set; }

        public string? Aoo { get; protected set; }

        public string? Amministrazione { get; protected set; }

        public bool Pubblicato { get; set; }

        public virtual void PubblicaAggiornamento(DatiPubblicazioneAggiornamento datiAggiornamento)
        {
            datiAggiornamento = datiAggiornamento ?? throw new ArgumentNullException(nameof(datiAggiornamento));

            this.AssertNonPubblicato();

            this.ApplyChange(new AggiornamentoPubblicatoEvent()
            {
                DatiAggiornamento = datiAggiornamento
            });
        }

        public IReadOnlyList<Email> Emails
        {
            get
            {
                return this._emails.AsReadOnly();
            }
        }

        public override IEnumerable<Pi3Exception> GetErrors()
        {
            var errors = new List<Pi3Exception>();

            if (string.IsNullOrWhiteSpace(this.Codice))
            {
                errors.Add(new MissingValuePi3Exception(nameof(this.Codice)));
            }

            if (this.Denominazione == null)
            {
                errors.Add(new MissingValuePi3Exception(nameof(this.Denominazione)));
            }

            return errors.AsReadOnly();
        }

        public void AssignId(string id)
        {
            if (!string.IsNullOrWhiteSpace(this.Id))
                throw new NotSupportedPi3Exception();

            this.ApplyChange(new IdAssignedEvent()
            {
                NewId = id
            });
        }


        public void ChangeDenominazione(string newDenominazione)
        {
            newDenominazione = newDenominazione ?? throw new ArgumentNullException(nameof(newDenominazione));

            this.AssertPubblicato();

            this.ApplyChange(new DenominazioneChangedEvent()
            {
                NewDenominazione = newDenominazione
            });
        }

        public void ChangeTipo(TipiEnum newTipo)
        {
            this.AssertPubblicato();
            
            this.ApplyChange(new TipoChangedEvent()
            {
                NewTipo = newTipo
            });
        }

        public void AddEmail(string id, bool? preferita = false, string? note = null)
        {
            id = id ?? throw new ArgumentNullException(nameof(id));

            this.AssertPubblicato();

            this.ApplyChange(new EmailAddedEvent()
            {
                IdEmail = id, 
                Preferita = preferita,
                Note = note
            });
        }

        public void RemoveEmail(string id)
        {
            id = id ?? throw new ArgumentNullException(nameof(id));

            this.AssertPubblicato();

            this.ApplyChange(new EmailRemovedEvent()
            {
                IdEmail = id
            });
        }

        public void ChangeEmailPreferita(string id, bool? preferita = false)
        {
            id = id ?? throw new ArgumentNullException(nameof(id));

            this.AssertPubblicato();

            if (!this._emails.Any(e => e.Id == id))
                throw new EmailNotFoundPi3Exception(id);

            this.ApplyChange(new EmailPreferitaChangedEvent()
            {
                IdEmail = id,
                Preferita = preferita
            });
        }

        public void ChangeEmailNote(string id, string? note = null)
        {
            id = id ?? throw new ArgumentNullException(nameof(id));

            this.AssertPubblicato();

            if (!this._emails.Any(e => e.Id == id))
                throw new EmailNotFoundPi3Exception(id);

            this.ApplyChange(new EmailNoteChangedEvent()
            {
                IdEmail = id,
                Note = note
            });
        }

        public void ChangeIndirizzo(Indirizzo? newIndirizzo)
        {
            newIndirizzo = newIndirizzo ?? throw new ArgumentNullException(nameof(newIndirizzo));

            this.AssertPubblicato();

            this.ApplyChange(new IndirizzoChangedEvent()
            {
                NewIndirizzo = newIndirizzo
            });
        }

        public void ChangeCodiceFiscale(string? newCodiceFiscale)
        {
            this.AssertPubblicato();

            this.ApplyChange(new CodiceFiscaleChangedEvent()
            {
                NewCodiceFiscale = newCodiceFiscale
            });
        }

        public void ChangePartitaIva(string? newPartitaIva)
        {
            this.AssertPubblicato();

            this.ApplyChange(new PartitaIvaChangedEvent()
            {
                NewPartitaIva = newPartitaIva
            });
        }

        public void ChangeUrlApiInteroperabilita(string? newUrlApiInteroperabilita)
        {
            this.AssertPubblicato();

            this.ApplyChange(new UrlApiInteroperabilitaChangedEvent()
            {
                NewUrlApiInteroperabilita = newUrlApiInteroperabilita
            });
        }

        public void ChangeAoo(string? newAoo)
        {
            this.AssertPubblicato();

            this.ApplyChange(new AOOChangedEvent()
            {
                NewAOO = newAoo
            });
        }

        public void ChangeAmministrazione(string? newAmministrazione)
        {
            this.AssertPubblicato();

            this.ApplyChange(new AmministrazioneChangedEvent()
            {
                NewAmministrazione = newAmministrazione
            });
        }

        #endregion

        #region Private Members

        protected List<Email> _emails = null!;

        protected void AssertPubblicato()
        {
            if (this.Pubblicato)
                throw new NotSupportedPi3Exception(ErrorDescriptions.ElementoPubblicato, ErrorDescriptions.ResourceManager);
        }

        protected void AssertNonPubblicato()
        {
            if (!this.Pubblicato)
                throw new NotSupportedPi3Exception(ErrorDescriptions.ElementoNonPubblicato, ErrorDescriptions.ResourceManager);
        }

        protected void Handle(CorrispondenteCreatoEvent @event)
        {
            this.Pubblicato = false;
            this._emails = new List<Email>();
            this.Id = @event.Id;
            this.Codice = @event.Codice;
            this.Denominazione = @event.Denominazione;
            this.Tipo = @event.Tipo;
            this.DataCreazione = @event.DataCreazione ?? DateTime.Now;
            this.DataUltimaModifica = @event.DataUltimaModifica ?? @event.DataCreazione ?? DateTime.Now;
        }

        protected void Handle(CorrispondentePubblicatoEvent @event)
        {
            this.Pubblicato = true;
            this._emails = new List<Email>();
            this.Id = @event.Id;
            this.Codice = @event.DatiPubblicazione.Codice;
            this.Denominazione = @event.DatiPubblicazione.Denominazione;
            this.Tipo = @event.DatiPubblicazione.Tipo;
            this.DataCreazione = DateTime.Now;
            this.DataUltimaModifica = DateTime.Now;
            this.Indirizzo = @event.DatiPubblicazione.Indirizzo;
            this.CodiceFiscale = @event.DatiPubblicazione.CodiceFiscale;
            this.PartitaIva = @event.DatiPubblicazione.PartitaIva;
            this.UrlApiInteroperabilita = @event.DatiPubblicazione.UrlApiInteroperabilita;
            this.Aoo = @event.DatiPubblicazione.AOO;
            this.Amministrazione = @event.DatiPubblicazione.Amministrazione;
            var emails = @event.DatiPubblicazione.Emails?.Select(e => new Email(e.Email, e.Preferita, e.Note));
            if (emails?.Any() ?? false)
                this._emails.AddRange(emails);
        }

        protected void Handle(IdAssignedEvent @event)
        {
            this.Id = @event.NewId;
        }

        protected virtual void Handle(DenominazioneChangedEvent @event)
        {
            this.Denominazione = @event.NewDenominazione;
        }

        protected virtual void Handle(TipoChangedEvent @event)
        {
            this.Tipo = @event.NewTipo;
        }

        protected void Handle(AggiornamentoPubblicatoEvent @event)
        {
            this.Pubblicato = true;
            this.Denominazione = @event.DatiAggiornamento.Denominazione;
            this.Indirizzo = @event.DatiAggiornamento.Indirizzo;
            this.CodiceFiscale = @event.DatiAggiornamento.CodiceFiscale;
            this.PartitaIva = @event.DatiAggiornamento.PartitaIva;
            this.UrlApiInteroperabilita = @event.DatiAggiornamento.UrlApiInteroperabilita;
            this.Aoo = @event.DatiAggiornamento.AOO;
            this.Amministrazione = @event.DatiAggiornamento.Amministrazione;
            this._emails.Clear();
            if (@event.DatiAggiornamento.Emails.Any())
                this._emails.AddRange(@event.DatiAggiornamento.Emails.Select(e => new Email(e.Email, e.Preferita, e.Note)));
        }

        protected virtual void Handle(EmailAddedEvent @event)
        {
            this._emails.Add(new Email(@event.IdEmail, @event.Preferita, @event.Note));

            this._emails
                .Where(e => e.Id != @event.IdEmail)
                .ToList()
                .ForEach(e => e.ChangePreferita(!@event.Preferita));
        }

        protected virtual void Handle(EmailRemovedEvent @event)
        {
            this._emails.RemoveAll(e => e.Id == @event.IdEmail);
        }

        protected virtual void Handle(EmailPreferitaChangedEvent @event)
        {
            var email = this._emails.First(e => e.Id == @event.IdEmail);

            email.ChangePreferita(@event.Preferita);

            this._emails
                .Where(e => e.Id != email.Id)
                .ToList()
                .ForEach(e => e.ChangePreferita(!@event.Preferita));
        }

        protected virtual void Handle(EmailNoteChangedEvent @event)
        {
            var email = this._emails.First(e => e.Id == @event.IdEmail);

            email.ChangeNote(@event.Note);
        }

        protected virtual void Handle(IndirizzoChangedEvent @event)
        {
            this.Indirizzo = @event.NewIndirizzo;
        }
        
        protected virtual void Handle(AOOChangedEvent @event)
        {
            this.Aoo = @event.NewAOO;
        }

        protected virtual void Handle(AmministrazioneChangedEvent @event)
        {
            this.Amministrazione = @event.NewAmministrazione;
        }

        protected virtual void Handle(CodiceFiscaleChangedEvent @event)
        {
            this.CodiceFiscale = @event.NewCodiceFiscale;
        }

        protected virtual void Handle(PartitaIvaChangedEvent @event)
        {
            this.PartitaIva = @event.NewPartitaIva;
        }

        protected virtual void Handle(UrlApiInteroperabilitaChangedEvent @event)
        {
            this.UrlApiInteroperabilita = @event.NewUrlApiInteroperabilita;
        }

        #endregion
    }
}
