// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Microsoft.Extensions.Logging;
using Pi3.Core.AggregateModels.CorrispondenteAggregate.Entities;
using Pi3.Core.AggregateModels.CorrispondenteAggregate.Events;
using Pi3.Core.AggregateModels.CorrispondenteAggregate.Exceptions;
using Pi3.Core.AggregateModels.ElementAggregate;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.CorrispondenteAggregate
{
    public abstract class Corrispondente : Element
    {
        #region Public Members

        public override bool IsValid()
        {
            return GetErrors().Any();
        }

        public virtual IEnumerable<Pi3Exception> GetErrors()
        {
            List<Pi3Exception> pi3Exceptions = new List<Pi3Exception>();

            if (this.CanalePreferenziale != null &&
                this.CanalePreferenziale.GetType() == typeof(CanalePreferenzialeMailCorrispondente) &&
                this.Email.Count == 0)
            {
                pi3Exceptions.Add(new EmailCorrispondenteRequredPi3Exception());
            }

            if (this.CanalePreferenziale != null && this.CanalePreferenziale.GetType() == typeof(CanalePreferenzialeInteroperabilitaCorrispondente))
            {
                if(this.Email.Count == 0)
                    pi3Exceptions.Add(new EmailCorrispondenteRequredPi3Exception());
                if (string.IsNullOrEmpty(this.CodiceAOO))
                    pi3Exceptions.Add(new CodiceAOORequredPi3Exception());
                if (string.IsNullOrEmpty(this.CodiceAmministrazione))
                    pi3Exceptions.Add(new CodiceAmministrazioneRequredPi3Exception());
            }

            if (!string.IsNullOrEmpty(this.Name.ToString()))
                pi3Exceptions.Add(new NameRequredPi3Exception());

            if (!string.IsNullOrEmpty(this.Description.ToString()))
                pi3Exceptions.Add(new DescriptionRequredPi3Exception());

            return pi3Exceptions;
        }

        public virtual void ChangeDescription(TextValue newDescription)
        {
            this.AssertCorrispondenteStoricizzato();

            this.ApplyChange(new DescriptionChangedEvent()
            {
                Id = this.Id,
                NewDescription = newDescription
            });
        }

        public virtual void ChangeName(TextValue newName)
        {
            throw new ChangeNameCorrispondenteNonSupportatoPi3Exception();
        }

        public string? IdRegistro { get; protected set; }

        public string? Codice { get; protected set; }

        public void SetCodice(string? codice)
        {
            this.AssertCorrispondenteStoricizzato();

            this.ApplyChange(new CodiceSettedEvent()
            {
                Id = this.Id,
                Codice = codice
            });
        }

        public string? IdOld { get; protected set; }

        public void SetIdOld(string? idOld)
        {
            this.AssertCorrispondenteStoricizzato();

            this.ApplyChange(new IdOldSettedEvent()
            {
                Id = this.Id,
                IdOld = idOld
            });
        }

        public string? DescriptionOld { get; protected set; }

        public void SetDescriptionOld(string? descriptionOld)
        {
            this.AssertCorrispondenteStoricizzato();

            this.ApplyChange(new DescriptionOldSettedEvent()
            {
                Id = this.Id,
                DescriptionOld = descriptionOld
            });
        }

        public string? InteropUrl { get; protected set; }

        public void SetInteropUrl(string? interopUrl)
        {
            this.AssertCorrispondenteStoricizzato();

            this.ApplyChange(new InteropUrlSettedEvent()
            {
                Id = this.Id,
                InteropUrl = interopUrl
            });
        }

        public CanalePreferenzialeCorrispondente? CanalePreferenziale { get; protected set; }

        public void SetCanalePreferenziale(string idCanalePreferenziale)
        {
            this.AssertCorrispondenteStoricizzato();

            this.ApplyChange(new CanalePreferenzialeSettedEvent()
            {
                Id = this.Id,
                IdCanalePreferenziale = idCanalePreferenziale
            });
        }

        public void SetCanalePreferenzialeMail(string idCanalePreferenziale)
        {
            this.AssertCorrispondenteStoricizzato();

            this.ApplyChange(new CanalePreferenzialeSettedEvent()
            {
                Id = this.Id,
                IdCanalePreferenziale = idCanalePreferenziale
            });
        }

        public void SetCanalePreferenzialeInteroperabilità(string idCanalePreferenziale)
        {
            this.AssertCorrispondenteStoricizzato();

            this.ApplyChange(new CanalePreferenzialeSettedEvent()
            {
                Id = this.Id,
                IdCanalePreferenziale = idCanalePreferenziale
            });
        }

        public string? CodiceAmministrazione { get; protected set; }

        public virtual void SetCodiceAmministrazione(string? newCodiceAmministrazione)
        {
            this.AssertCorrispondenteStoricizzato();

            this.ApplyChange(new CodiceAmministrazioneSettedEvent()
            {
                Id = this.Id,
                NewCodiceAmministrazione = newCodiceAmministrazione
            });
        }

        public string? CodiceAOO { get; protected set; }

        public virtual void SetCodiceAOO(string? newCodiceAOO)
        {
            this.AssertCorrispondenteStoricizzato();

            this.ApplyChange(new CodiceAOOSettedEvent()
            {
                Id = this.Id,
                NewCodiceAOO = newCodiceAOO
            });
        }

        public IReadOnlyList<EmailCorrispondente> Email
        {
            get
            {
                return this._email.AsReadOnly();
            }
        }
        public void AddEmail(string idEmail, string indirizzoEmail, TextValue note, bool principale = false)
        {
            this.AssertCorrispondenteStoricizzato();

            this.AssertEmail(indirizzoEmail);

            if(this._email == null || this._email.Count == 0)
                principale = true;

            this.ApplyChange(new EmailAddedEvent()
            {
                Id = this.Id,
                IdEmail = idEmail,
                IndirizzoEmail = indirizzoEmail,
                Note = note,
                Principale = principale
            });
        }

        public void RemoveEmail(string id)
        {
            this.AssertCorrispondenteStoricizzato();

            this.AssertEmailPrincipale(id);

            this.ApplyChange(new EmailRemovedEvent()
            {
                Id = this.Id,
                IdEmail = id
            });
        }

        public void ModifyEmail(string id, string indirizzoEmail, TextValue note)
        {
            this.AssertCorrispondenteStoricizzato();

            this.AssertEmail(indirizzoEmail);

            this.ApplyChange(new EmailModifiedEvent()
            {
                Id = this.Id,
                IdEmail = id,
                IndirizzoEmail = indirizzoEmail,
                Note = note
            });
        }

        public void SetEmailPrincipale(string indirizzoEmail)
        {
            this.AssertCorrispondenteStoricizzato();

            this.ApplyChange(new EmailPrincipaleSettedEvent()
            {
                Id = this.Id,
                IndirizzoEmail = indirizzoEmail
            });
        }

        public DateTime? DataFine { get; protected set; } = null;

        public void SetDataFine(DateTime? dataFine)
        {
            this.AssertCorrispondenteStoricizzato();

            this.ApplyChange(new DataFineSettedEvent()
            {
                Id = this.Id,
                NewDataFine= dataFine
            });
        }

        public bool? RubricaComune { get; protected set; }

        public void SetRubricaComune(bool rubricaComune)
        {
            this.AssertCorrispondenteStoricizzato();

            this.ApplyChange(new RubricaComuneSettedEvent()
            {
                Id = this.Id,
                NewRubricaComune = rubricaComune
            });
        }

        public string? RubricaEsterna { get; protected set; }

        public void SetRubricaEsterna(string? rubricaEsterna)
        {
            this.AssertCorrispondenteStoricizzato();

            this.ApplyChange(new RubricaEsternaSettedEvent()
            {
                Id = this.Id,
                NewRubricaEsterna = rubricaEsterna
            });
        }

        #endregion

        #region Private Members

        protected List<EmailCorrispondente> _email;

        protected void AssertEmail(string indirizzoEmail)
        {
            string pattern = "^(([a-zA-Z0-9_\\-\\.]+)@([a-zA-Z0-9_\\-\\.]+)\\.([a-zA-Z]{2,5}){1,25})+([;](([a-zA-Z0-9_\\-\\.]+)@([a-zA-Z0-9_\\-\\.]+)\\.([a-zA-Z]{2,5}){1,25})+)*$";
            if (string.IsNullOrEmpty(indirizzoEmail) 
                || !System.Text.RegularExpressions.Regex.Match(
                            indirizzoEmail.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()),
                            pattern,
                            System.Text.RegularExpressions.RegexOptions.None,
                            TimeSpan.FromMilliseconds(100)).Success)
                throw new EmailAddressNotValidPi3Exception(indirizzoEmail);
        }
        protected void AssertEmailPrincipale(string idEmail)
        {
            if (this._email.Any(e => e.Id == idEmail && e.Principale) && this._email.Count > 1)
                throw new EmailNotBeRemovedPi3Exception();
        }

        protected void AssertCorrispondenteStoricizzato()
        {
            if(this.DataFine != null)
                throw new CorrispondenteStoricizzatoPi3Exception();
        }

        protected virtual void Handle(DescriptionChangedEvent @event)
        {
            this.Description = @event.NewDescription;
        }

        protected virtual void Handle(CodiceAmministrazioneSettedEvent @event)
        {
            this.CodiceAmministrazione = @event.NewCodiceAmministrazione;
        }

        protected virtual void Handle(CodiceAOOSettedEvent @event)
        {
            this.CodiceAOO = @event.NewCodiceAOO;
        }

        protected virtual void Handle(EmailAddedEvent @event)
        {
            if (@event.Principale)
                foreach (var e in this._email)
                    e.SetPrincipale(false);

            this._email.Add(new EmailCorrispondente(@event.IdEmail, @event.IndirizzoEmail, @event.Note, @event.Principale));
        }

        protected virtual void Handle(EmailRemovedEvent @event)
        {
            if (this._email.Any(e => e.Id == @event.IdEmail))
                this._email.Remove(this._email.First(e => e.Id == @event.IdEmail));
        }

        protected virtual void Handle(EmailModifiedEvent @event)
        {
            EmailCorrispondente email = this._email.FirstOrDefault(e => e.Id == @event.IdEmail)!;
            if (email! != null!)
                email.Modify(@event.IndirizzoEmail, @event.Note);
        }

        protected virtual void Handle(EmailPrincipaleSettedEvent @event)
        {
            EmailCorrispondente oldEmailPrincipale = this._email.FirstOrDefault(e => e.Principale)!;
            if (oldEmailPrincipale! != null!)
                oldEmailPrincipale.SetPrincipale(false);

            EmailCorrispondente newEmailPrincipale = this._email.FirstOrDefault(e => e.IndirizzoEmail == @event.IndirizzoEmail)!;
            if (newEmailPrincipale! != null!)
                newEmailPrincipale.SetPrincipale(true);
        }

        protected virtual void Handle(CanalePreferenzialeSettedEvent @event)
        {
            this.CanalePreferenziale = new CanalePreferenzialeCorrispondente(@event.IdCanalePreferenziale);
        }

        protected virtual void Handle(CanalePreferenzialeMailSettedEvent @event)
        {
            this.CanalePreferenziale = new CanalePreferenzialeMailCorrispondente(@event.IdCanalePreferenziale);
        }

        protected virtual void Handle(CanalePreferenzialeInteroperabilitaSettedEvent @event)
        {
            this.CanalePreferenziale = new CanalePreferenzialeInteroperabilitaCorrispondente(@event.IdCanalePreferenziale);
        }

        protected virtual void Handle(DataFineSettedEvent @event)
        {
            this.DataFine = (@event.NewDataFine);
        }

        protected virtual void Handle(RubricaComuneSettedEvent @event)
        {
            this.RubricaComune = (@event.NewRubricaComune);
        }

        protected virtual void Handle(RubricaEsternaSettedEvent @event)
        {
            this.RubricaEsterna = (@event.NewRubricaEsterna);
        }
        protected virtual void Handle(CodiceSettedEvent @event)
        {
            this.Codice = (@event.Codice);
        }

        protected virtual void Handle(IdOldSettedEvent @event)
        {
            this.IdOld = (@event.IdOld);
        }

        protected virtual void Handle(DescriptionOldSettedEvent @event)
        {
            this.DescriptionOld = (@event.DescriptionOld);
        }

        protected virtual void Handle(InteropUrlSettedEvent @event)
        {
            this.InteropUrl = (@event.InteropUrl);
        }
        #endregion
    }
}