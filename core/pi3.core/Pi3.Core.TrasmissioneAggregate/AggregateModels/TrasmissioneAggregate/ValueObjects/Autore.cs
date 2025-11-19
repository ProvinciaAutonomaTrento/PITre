// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.TrasmissioneAggregate.ValueObjects
{
    public class Autore : ValueObject
    {
        [Required(AllowEmptyStrings = false)]
        public string IdUtente { get; init; }

        public string? UserId { get; init; } = null;

        public string? Cognome { get; init; } = null;

        public string? Nome { get; init; } = null;

        [Required(AllowEmptyStrings = false)]
        public string IdGruppo { get; init; }

        public string? CodiceGruppo { get; init; } = null;

        public string? DescrizioneGruppo { get; init; } = null;

        public string? IdUtenteDelegato { get; init; } = null;

        public string? UserIdDelegato { get; init; } = null;

        public string? CognomeDelegato { get; init; } = null;

        public string? NomeDelegato { get; init; } = null;
    }
}
