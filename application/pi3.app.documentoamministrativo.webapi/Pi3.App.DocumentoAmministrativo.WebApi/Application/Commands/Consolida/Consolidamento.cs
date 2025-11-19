// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.App.DocumentoAmministrativo.WebApi.Binders;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Consolida
{
    [ResourceSwaggerSchema("Consolidamento_Head")]
    public class Consolidamento
    {
        [ResourceSwaggerSchema("Consolidamento_Stato")]
        public StatiConsolidamentoEnum Stato { get; init; }

        [ResourceSwaggerSchema("Consolidamento_Data")]
        public DateTime Data { get; init; }
        [ResourceSwaggerSchema("Consolidamento_IdAutore")]

        public string IdAutore { get; init; }
    }
}
