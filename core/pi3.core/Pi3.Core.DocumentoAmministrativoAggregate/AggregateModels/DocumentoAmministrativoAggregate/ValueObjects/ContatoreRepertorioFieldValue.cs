// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.AggregateModels.ElementAggregate.ValueObjects;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects
{
    public class ContatoreRepertorioFieldValue : ElementFieldValue
    {
        public ContatoreRepertorioFieldValue() : base()
        {
        }

        public ContatoreRepertorioFieldValue(string idRegistro, long value, DateTime? dataAnnullamento = null)
        {
            IdRegistro = idRegistro;
            Value = value;
            DataAnnullamento = dataAnnullamento;
        }

        public ContatoreRepertorioFieldValue(string idRegistro, bool conta, bool resetInizioAnno)
        {
            IdRegistro = idRegistro;
            Conta = conta;
            ResetInizioAnno = resetInizioAnno;
        }

        public string IdRegistro { get; init; }

        public bool? ResetInizioAnno { get; init; } = true;

        public bool? Conta { get; init; } = false;

        public long? Value { get; init; } = null;
        public DateTime? DataAnnullamento { get; init; } = null;

        public override string ToString()
        {
            if (Value.HasValue)
                return Value.ToString()!;
            else
                return string.Empty;
        }
    }
}
