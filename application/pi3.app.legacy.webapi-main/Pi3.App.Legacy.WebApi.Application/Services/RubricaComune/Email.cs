// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Services.RubricaComune
{
    public class Email : ValueObject
    {
        public string Indirizzo { get; init; } = null!;

        public bool? Preferita { get; init; }

        public string? Note { get; init; }
    }
}
