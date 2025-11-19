// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects
{
    public class OggettoDelDocumento : ValueObject
    {
        public OggettoDelDocumento()
        { }

        [Required(AllowEmptyStrings = false)]
        public TextValue Descrizione { get; init; } = null;

        public string? Id { get; init; } = null;
    }
}
