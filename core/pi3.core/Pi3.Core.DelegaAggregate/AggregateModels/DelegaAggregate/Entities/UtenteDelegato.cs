// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.DelegaAggregate.Entities
{
    public class UtenteDelegato : Entity<string>
    {
        #region Public Members

        internal UtenteDelegato(string id, string userId, string cognome, string nome)
        {
            Id = id;
            UserId = userId;
            Cognome = cognome;
            Nome = nome;
        }

        public string? UserId { get; protected set; }

        public string? Cognome { get; protected set; }

        public string? Nome { get; protected set; }

        #endregion

        #region Private Members

        #endregion
    }
}
