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
    public class AddTrasmissioneSingolaUtenteAction : ValueObject
    {
        public AddTrasmissioneSingolaUtenteAction()
        { }

        [Required(AllowEmptyStrings = false)]
        public string IdRagioneTrasmissione { get; init; }

        public string? NomeRagioneTrasmissione { get; init; } = null;

        [Required(AllowEmptyStrings = false)]
        public string IdUtenteDestinatario { get; init; }

        public string? UserIdDestinatario { get; init; } = null;

        public string? CognomeDestinatario { get; init; } = null;

        public string? NomeDestinatario { get; init; } = null;
    }
}
