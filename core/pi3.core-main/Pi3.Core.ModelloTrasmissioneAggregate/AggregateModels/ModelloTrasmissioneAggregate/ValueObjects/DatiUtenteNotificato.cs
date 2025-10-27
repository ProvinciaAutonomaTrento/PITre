// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.ModelloTrasmissioneAggregate.ValueObjects
{
    public class DatiUtenteNotificato : ValueObject
    {
        [Required(AllowEmptyStrings = false)]
        public string IdUtente { get; init; }

        public string? UserId { get; init; } = null;

        public string? Cognome { get; init; } = null;

        public string? Nome { get; init; } = null;

        public bool? CediDiritti { get; init; } = null;
    }
}
