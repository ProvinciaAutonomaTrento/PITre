// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.ModelloTrasmissioneAggregate.Entities
{
    public class AutorePersona : Autore
    {
        internal AutorePersona(string id, string? userId = null, string? cognome = null, string? nome = null)
            : base(id)
        {
            UserId = userId;
            Cognome = cognome;
            Nome = nome;
        }

        public string? UserId { get; protected set; } = null;
        public string? Cognome { get; protected set; } = null;
        public string? Nome { get; protected set; } = null;
    }

}
