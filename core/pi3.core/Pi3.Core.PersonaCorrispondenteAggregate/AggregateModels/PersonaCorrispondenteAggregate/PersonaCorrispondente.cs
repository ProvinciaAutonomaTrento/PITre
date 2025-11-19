// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.Extensions.Logging;
using Pi3.Core.AggregateModels.CorrispondenteAggregate;
using Pi3.Core.AggregateModels.CorrispondenteAggregate.Entities;
using Pi3.Core.AggregateModels.CorrispondenteAggregate.ValueObjects;
using Pi3.Core.AggregateModels.ElementAggregate.Events;
using Pi3.Core.AggregateModels.PersonaCorrispondenteAggregate.Events;
using Pi3.Core.AggregateModels.PersonaCorrispondenteAggregate.Exceptions;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace Pi3.Core.AggregateModels.PersonaCorrispondenteAggregate
{
    public class PersonaCorrispondente : Corrispondente
    {
        #region Public Members
        
        public PersonaCorrispondente(string idTenant, DateTime creationDate, TextValue name, TextValue description, string nome, string cognome, string? idRegistro = null)
            : this(null, idTenant, creationDate, name, description, nome, cognome, idRegistro)
        {
        }

        public PersonaCorrispondente(
            string id,
            string idTenant,
            DateTime creationDate,
            TextValue name,
            TextValue description,        
            string nome, 
            string cognome,
            string? idRegistro)
        {
            this.ApplyChange(new PersonaCorrispondenteCreatedEvent()
            {
                Id = id,
                IdTenant = idTenant,
                TypeName = nameof(PersonaCorrispondente),
                CreationDate = creationDate,
                Name = name,
                Description = description,
                IdRegistro = idRegistro,
                Nome = nome,
                Cognome = cognome
            });
        }

        public string Nome { get; protected set; }

        public void ChangeNome(string nome)
        {
            this.AssertCorrispondenteStoricizzato();

            this.ApplyChange(new NomeChangedEvent()
            {
                Id = this.Id,
                NewNome = nome
            });
        }

        public string Cognome { get; protected set; }

        public void ChangeCognome(string cognome)
        {
            this.AssertCorrispondenteStoricizzato();

            this.ApplyChange(new CognomeChangedEvent()
            {
                Id = this.Id,
                NewCognome = cognome
            });
        }

        public string? LuogoNascita { get; protected set; }

        public void SetLuogoNascita(string luogoNascita)
        {
            this.AssertCorrispondenteStoricizzato();

            this.ApplyChange(new LuogoNascitaSettedEvent()
            {
                Id = this.Id,
                NewLuogoNascita = luogoNascita
            });
        }

        public DateTime? DataNascita { get; protected set; }

        public void SetDataNascita(DateTime? dataNascita)
        {
            this.AssertCorrispondenteStoricizzato();

            this.ApplyChange(new DataNascitaSettedEvent()
            {
                Id = this.Id,
                NewDataNascita = dataNascita
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

        public string? Titolo { get; protected set; }

        public void SetTitolo(string titolo)
        {
            this.AssertCorrispondenteStoricizzato();

            this.ApplyChange(new TitoloSettedEvent()
            {
                Id = this.Id,
                Titolo = titolo
            });
        }

        #endregion

        #region Private Members

        protected PersonaCorrispondente() : base()
        { }

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
                    arrTaxCode[k] = x.ToString()[0];
            }

            Regex rgx = new Regex(@"^[A-Z]{6}[0-9]{2}[A-Z][0-9]{2}[A-Z][0-9]{3}[A-Z]$",
                            RegexOptions.None,
                            TimeSpan.FromMilliseconds(100));

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
                    c = listControl.Substring(x, 1)[0];
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

            const int character = 11;
            Regex pregex = new Regex("^\\d{" + character.ToString() + "}$",
                            RegexOptions.None,
                            TimeSpan.FromMilliseconds(100));

            if(partitaIva.Length != character || !pregex.Match(partitaIva).Success || (int.Parse(partitaIva.Substring(0, 7)) == 0)
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

        public virtual void Handle(PersonaCorrispondenteCreatedEvent @event)
        {
            base.Handle((ElementCreatedEvent)@event);

            this._email = new List<EmailCorrispondente>();
            this.IdRegistro = @event.IdRegistro;
            this.Nome = @event.Nome;
            this.Cognome = @event.Cognome;
        }

        protected virtual void Handle(NomeChangedEvent @event)
        {
            this.Nome = @event.NewNome;
        }

        protected virtual void Handle(CognomeChangedEvent @event)
        {
            this.Cognome = @event.NewCognome;
        }

        protected virtual void Handle(LuogoNascitaSettedEvent @event)
        {
            this.LuogoNascita = @event.NewLuogoNascita;
        }

        protected virtual void Handle(DataNascitaSettedEvent @event)
        {
            this.DataNascita = @event.NewDataNascita;
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

        protected virtual void Handle(NoteSettedEvent @event)
        {
            this.Note = @event.NewNote;
        }

        protected virtual void Handle(TitoloSettedEvent @event)
        {
            this.Titolo = @event.Titolo;
        }
        #endregion
    }
}
