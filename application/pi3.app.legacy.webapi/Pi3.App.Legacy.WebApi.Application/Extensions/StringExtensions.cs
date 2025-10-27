// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Infrastructure.EF.Extensions
{
    public static class StringExtensions
    {
        public static TipologiaFlussoEnum? AsTipologiaFlusso(this string tipoProto)
        {
            switch (tipoProto)
            {
                case "A":
                    return TipologiaFlussoEnum.E;
                case "P":
                    return TipologiaFlussoEnum.U;
                case "I":
                    return TipologiaFlussoEnum.I;
                default:
                    return null;
            }
        }
    }
}
