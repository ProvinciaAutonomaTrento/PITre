// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Queries;
using Pi3.App.DocumentoAmministrativo.WebApi.Binders;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects;
using Pi3.Core.SeedWork;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Consolida
{
    [ResourceSwaggerSchema("ConsolidamentoCommandResponse")]
    public class ConsolidamentoCommandResponse : ValueObject {
        [ResourceSwaggerSchema("DocumentoAmministrativo")]
        public Documento DocumentoAmministrativo { get; init; }

        [ResourceSwaggerSchema("listaServiziApi")]
        public IEnumerable<Link> Links { get; set; }
    }
    public class ConsolidamentoCommand: IRequest<ConsolidamentoCommandResponse>
    {
        public string Id { get; set; }
        public StatiConsolidamentoEnum Stato { get; init; }

        public DateTime Data { get; init; }

        public string IdAutore { get; init; }
    }
}
