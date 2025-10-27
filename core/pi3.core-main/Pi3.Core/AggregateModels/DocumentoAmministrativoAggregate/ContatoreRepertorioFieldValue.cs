// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.AggregateModels.ElementAggregate;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate
{
    public class ContatoreRepertorioFieldValue : ElementFieldValue
    {
        public ContatoreRepertorioFieldValue() : base()
        {
        }

        public ContatoreRepertorioFieldValue(string idRegistro, long value)
        {   
            this.IdRegistro = idRegistro;
            this.Value = value;
        }

        public ContatoreRepertorioFieldValue(string idRegistro, bool conta, bool resetInizioAnno)
        {
            this.IdRegistro = idRegistro;
            this.Conta = conta;
            this.ResetInizioAnno = resetInizioAnno;
        }

        public string IdRegistro { get; init; }

        public bool? ResetInizioAnno { get; init; } = true;

        public bool? Conta { get; init; } = false;

        public long? Value { get; init; } = null;

        public override string ToString()
        {
            if (this.Value.HasValue)
                return this.Value.ToString();
            else
                return null;
        }
    }
}
