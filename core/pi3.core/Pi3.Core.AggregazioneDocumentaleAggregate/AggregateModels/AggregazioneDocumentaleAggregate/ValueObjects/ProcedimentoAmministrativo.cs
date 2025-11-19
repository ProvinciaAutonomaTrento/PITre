// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects
{
    public class ProcedimentoAmministrativo : ValueObject
    {
        public ProcedimentoAmministrativo()
        { }

        [Required(AllowEmptyStrings = false)]
        public string MateriaArgomentoStruttura { get; init; }


        [Required(AllowEmptyStrings = false)]
        public string Procedimento { get; init; }

        public string? CatalogoProcedimenti { get; init; }

        [Required, MinLength(1)]
        public IReadOnlyList<Fase> Fasi { get; init; }
    }
}
