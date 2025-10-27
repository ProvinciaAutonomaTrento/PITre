// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.AggregateModels.CorrispondenteAggregate;
using Pi3.Core.AggregateModels.CorrispondenteAggregate.Entities;
using Pi3.Core.AggregateModels.CorrispondenteAggregate.ValueObjects;
using Pi3.Core.AggregateModels.ElementAggregate;
using Pi3.Core.AggregateModels.UOCorrispondenteAggregate.Events;
using Pi3.Core.AggregateModels.UOCorrispondenteAggregate.Exceptions;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.UOCorrispondenteAggregate
{
    public class UOCorrispondente : Corrispondente
    {
        #region Public Members

        public UOCorrispondente(string idTenant, DateTime creationDate, TextValue name, TextValue description, string? idRegistro = null)
            : this(null, idTenant, creationDate, name, description, idRegistro)
        {
        }

        public UOCorrispondente(
            string id,
            string idTenant,
            DateTime creationDate,
            TextValue name,
            TextValue description,
            string? idRegistro)
        {
            this.ApplyChange(new UOCorrispondenteCreatedEvent()
            {
                Id = id,
                IdTenant = idTenant,
                TypeName = nameof(UOCorrispondente),
                CreationDate = creationDate,
                Name = name,
                Description = description,
                IdRegistro = idRegistro
            });
        }

        public IndirizzoCorrispondente? Indirizzo { get; protected set; }

        public void ChangeIndirizzo(IndirizzoCorrispondente indirizzo)
        {
            this.AssertCorrispondenteStoricizzato();

            indirizzo = indirizzo ?? throw new ArgumentNullException(nameof(indirizzo));

            Validator.ValidateObject(indirizzo, new ValidationContext(indirizzo), true);

            this.ApplyChange(new IndirizzoChangedEvent()
            {
                Id = this.Id,
                NewIndirizzo = indirizzo
            });
        }

        public string? CodiceFiscale { get; protected set; }

        public void SetCodiceFiscale(string codiceFiscale)
        {
            this.AssertCorrispondenteStoricizzato();

            this.AssertCodiceFiscale(codiceFiscale);

            this.ApplyChange(new CodiceFiscaleSettedEvent()
            {
                Id = this.Id,
                NewCodiceFiscale = codiceFiscale
            });
        }

        public string? PartitaIva { get; protected set; }

        public void SetPartitaIva(string partitaIva)
        {
            this.AssertCorrispondenteStoricizzato();

            this.AssertPartitaIva(partitaIva);

            this.ApplyChange(new PartitaIvaSettedEvent()
            {
                Id = this.Id,
                NewPartitaIva = partitaIva
            });
        }

        public string? CodiceIpa { get; protected set; }

        public void SetCodiceIpa(string codiceIpa)
        {
            this.AssertCorrispondenteStoricizzato();

            this.ApplyChange(new CodiceIpaSettedEvent()
            {
                Id = this.Id,
                NewCodiceIpa = codiceIpa
            });
        }

        public string? Note { get; protected set; }

        public void SetNote(string note)
        {
            this.AssertCorrispondenteStoricizzato();

            this.ApplyChange(new NoteSettedEvent()
            {
                Id = this.Id,
                NewNote = note
            });
        }

        #endregion

        #region Private Members

        protected void AssertCodiceFiscale(string codiceFiscale)
        {
            if (string.IsNullOrEmpty(codiceFiscale))
                return;

            codiceFiscale = codiceFiscale.Replace(" ", "");
            bool result = false;
            const int character = 16;
            const string omocode = "LMNPQRSTUV";
            const string listControl = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            int[] listEquivalent = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25 };
            int[] listaUnequal = { 1, 0, 5, 7, 9, 13, 15, 17, 19, 21, 2, 4, 18, 20, 11, 3, 6, 8, 12, 14, 16, 10, 22, 25, 24, 23 };

            result = (string.IsNullOrEmpty(codiceFiscale) || codiceFiscale.Length != character);
            if (result)
                throw new CodiceFiscaleNotValidPi3Exception();

            codiceFiscale = codiceFiscale.ToUpper();
            char[] arrTaxCode = codiceFiscale.ToCharArray();

            for (int k = 6; k < 15; k++)
            {
                if ((k == 8) || (k == 11))
                    continue;
                int x = (omocode.IndexOf(arrTaxCode[k]));
                if (x != -1)
                    arrTaxCode[k] = x.ToString().ToCharArray()[0];
            }

            Regex rgx = new Regex(@"^[A-Z]{6}[0-9]{2}[A-Z][0-9]{2}[A-Z][0-9]{3}[A-Z]$");
            Match m = rgx.Match(new string(arrTaxCode));
            result = m.Success;
            if (!result)
                throw new CodiceFiscaleNotValidPi3Exception();

            int somma = 0;
            arrTaxCode = codiceFiscale.ToCharArray();
            for (int i = 0; i < 15; i++)
            {
                char c = arrTaxCode[i];
                int x = "0123456789".IndexOf(c);
                if (x != -1)
                    c = listControl.Substring(x, 1).ToCharArray()[0];
                x = listControl.IndexOf(c);
                if ((i % 2) == 0)
                    x = listaUnequal[x];
                else
                    x = listEquivalent[x];
                somma += x;
            }
            result = (listControl.Substring(somma % 26, 1) == codiceFiscale.Substring(15, 1));
            if (!result)
                throw new CodiceFiscaleNotValidPi3Exception();
        }

        protected void AssertPartitaIva(string partitaIva)
        {
            if (string.IsNullOrEmpty(partitaIva))
                return;

            bool result = false;
            const int character = 11;
            Regex pregex = new Regex("^\\d{" + character.ToString() + "}$");

            if (partitaIva.Length != character || !pregex.Match(partitaIva).Success || (int.Parse(partitaIva.Substring(0, 7)) == 0)
                || !((int.Parse(partitaIva.Substring(7, 3)) >= 0) && (int.Parse(partitaIva.Substring(7, 3)) < 201)))
                throw new PartitaIvaNotValidPi3Exception();

            int sum = 0;
            for (int i = 0; i < character - 1; i++)
            {
                int j = int.Parse(partitaIva.Substring(i, 1));
                if ((i + 1) % 2 == 0)
                {
                    j *= 2;
                    char[] c = j.ToString("00").ToCharArray();
                    sum += int.Parse(c[0].ToString());
                    sum += int.Parse(c[1].ToString());
                }
                else
                    sum += j;
            }
            if ((sum.ToString("00").Substring(1, 1).Equals("0")) && (!partitaIva.Substring(10, 1).Equals("0")))
                throw new PartitaIvaNotValidPi3Exception();

            sum = int.Parse(partitaIva.Substring(10, 1)) + int.Parse(sum.ToString("00").Substring(1, 1));
            if (!sum.ToString("00").Substring(1, 1).Equals("0"))
                throw new PartitaIvaNotValidPi3Exception();
        }

        public virtual void Handle(UOCorrispondenteCreatedEvent @event)
        {
            base.Handle((ElementCreatedEvent)@event);

            this._email = new List<EmailCorrispondente>();
            this.IdRegistro = @event.IdRegistro;
        }

        protected virtual void Handle(IndirizzoChangedEvent @event)
        {
            this.Indirizzo = @event.NewIndirizzo;
        }

        protected virtual void Handle(CodiceFiscaleSettedEvent @event)
        {
            this.CodiceFiscale = @event.NewCodiceFiscale;
        }

        protected virtual void Handle(PartitaIvaSettedEvent @event)
        {
            this.PartitaIva = @event.NewPartitaIva;
        }

        protected virtual void Handle(CodiceIpaSettedEvent @event)
        {
            this.CodiceIpa = @event.NewCodiceIpa;
        }

        protected virtual void Handle(NoteSettedEvent @event)
        {
            this.Note = @event.NewNote;
        }
        #endregion
    }
}
