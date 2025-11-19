// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate.Extensions
{
    public static class TipologiaFlussoEnumExtensions
    {
        public static string? AsTipoProto(this TipologiaFlussoEnum? tipologiaFlusso)
        {
            switch (tipologiaFlusso)
            {
                case TipologiaFlussoEnum.E:
                    return "A";
                case TipologiaFlussoEnum.U:
                    return "P";
                case TipologiaFlussoEnum.I:
                    return "I";
                default:
                    return null;
            }
        }
    }
}
