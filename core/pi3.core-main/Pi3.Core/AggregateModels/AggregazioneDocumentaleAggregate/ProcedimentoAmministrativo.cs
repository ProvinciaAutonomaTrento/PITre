// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate
{
    public abstract class Fase : ValueObject
    {
        public Fase()
        {
        }

        [Required]
        public DateTime DataInizioFase { get; init; }

        public DateTime? DataFineFase { get; init; }
    }

    public class FasePreparatoria : Fase
    {
        public FasePreparatoria() : base()
        {
        }
    }

    public class FaseIstruttoria : Fase
    {
        public FaseIstruttoria() : base()
        {
        }
    }

    public class FaseConsultiva : Fase
    {
        public FaseConsultiva() : base()
        {
        }
    }

    public class FaseDecisoriaODeliberativa : Fase
    {
        public FaseDecisoriaODeliberativa() : base()
        {
        }
    }

    public class FaseIntegrazioneEfficacia : Fase
    {
        public FaseIntegrazioneEfficacia() : base()
        {
        }
    }

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
