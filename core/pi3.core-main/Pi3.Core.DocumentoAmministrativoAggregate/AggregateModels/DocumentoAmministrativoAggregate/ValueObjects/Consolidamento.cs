// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects
{
    public enum StatiConsolidamentoEnum
    {
        /// <summary>
        /// Consolidato il documento in azioni fondamentali che determinano un incremento o modifica delle versioni
        /// </summary>
        Livello1 = 1,

        /// <summary>
        /// Consolidato il documento nei suoi metadati fondamentali
        /// </summary>
        Livello2 = 2,
    }

    public class Consolidamento : ValueObject
    {
        public Consolidamento()
        { }

        [Required]
        public StatiConsolidamentoEnum Stato { get; init; }

        [Required]
        public DateTime Data { get; init; }

        [Required]
        public Autore Autore { get; init; }
    }
}
