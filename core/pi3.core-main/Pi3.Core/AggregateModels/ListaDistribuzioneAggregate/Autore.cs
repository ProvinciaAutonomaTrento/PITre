// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pi3.Core.AggregateModels.ListaDistribuzioneAggregate;

namespace Pi3.Core.AggregateModels.ListaDistribuzioneAggregate
{
    public class AutorePersona : Autore
    {
        internal AutorePersona(string id, string? userId = null, string? cognome = null, string? nome = null)
            : base(id)
        {
            this.UserId = userId;
            this.Cognome = cognome;
            this.Nome = nome;
        }

        public string? UserId { get; protected set; } = null;
        public string? Cognome { get; protected set; } = null;
        public string? Nome { get; protected set; } = null;
    }

    public class AutoreGruppo : Autore
    {
        internal AutoreGruppo(string id, string? idGruppo = null, TextValue? descrizione = null) : base(id)
        {
            this.IdGruppo = idGruppo;
            this.Descrizione = descrizione;
        }

        public string IdGruppo { get; protected set; } = null;
        public TextValue? Descrizione { get; protected set; } = null;

    }

    public abstract class Autore : Entity<string>
    {
        internal Autore(string id)
        {
            this.Id = id;
        }

        public string Id { get; protected set; }
    }
}

