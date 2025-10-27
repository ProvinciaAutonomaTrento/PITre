// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.documento;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.ConsolidateDocumentById
{
    internal static class TypeExtensions
    {
        internal static StatiConsolidamentoEnum ToStatiConsolidamentoEnum(this DocumentConsolidationStateEnum x)
        {
            if (x.Equals(DocumentConsolidationStateEnum.Step1)) return StatiConsolidamentoEnum.Livello1;
            if (x.Equals(DocumentConsolidationStateEnum.Step2)) return StatiConsolidamentoEnum.Livello2;
            return default;
        }

        internal static DocumentConsolidationStateEnum ToDocumentConsolidationState(this StatiConsolidamentoEnum x)
        {
            if (x.Equals(StatiConsolidamentoEnum.Livello1)) return DocumentConsolidationStateEnum.Step1;
            if (x.Equals(StatiConsolidamentoEnum.Livello2)) return DocumentConsolidationStateEnum.Step2;
            return DocumentConsolidationStateEnum.None;
        }
    }
}
