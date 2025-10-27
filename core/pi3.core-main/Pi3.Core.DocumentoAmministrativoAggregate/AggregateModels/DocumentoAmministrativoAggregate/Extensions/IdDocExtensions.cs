// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Metadati;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Extensions
{
    internal static class IdDocExtensions
    {
        public static IdDocType AsIdDocType(this IdDoc idDoc)
        {
            idDoc = idDoc ?? throw new ArgumentNullException(nameof(idDoc));

            var type = new IdDocType();

            type.Identificativo = idDoc!.Identiticativo;

            if (idDoc!.ImprontaCrittograficaDelDocumento != null!)
            {
                type.ImprontaCrittograficaDelDocumento = new ImprontaCrittograficaDelDocumentoType()
                {
                    Impronta = idDoc.ImprontaCrittograficaDelDocumento!.Impronta,
                    Algoritmo = idDoc.ImprontaCrittograficaDelDocumento!.Algoritmo
                };
            }

            type.Segnatura = idDoc!.Segnatura;

            return type;
        }
    }
}
