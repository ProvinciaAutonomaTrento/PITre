// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.TrasmissioneAggregate
{

    public class UtenteDestinatario : Entity<string>
    {
        #region Public Members

        internal UtenteDestinatario(string id, string? userId = null, string? cognome = null, string nome = null)
        {
            this.Id = id;
            this.UserId = userId;
            this.Cognome = cognome;
            this.Nome = nome;
        }

        public string? UserId { get; protected set; } = null;

        public string? Cognome { get; protected set; } = null;

        public string? Nome { get; protected set; } = null;


        #endregion

        #region Private Members

        #endregion
    }
}
