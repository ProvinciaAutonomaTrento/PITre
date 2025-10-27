// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Queries;
using Pi3.App.DocumentoAmministrativo.WebApi.Binders;
using Pi3.Core.SeedWork;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Annullamento
{
    [ResourceSwaggerSchema("AnnullamentoCommandResponse")]
    public class AnnullamentoCommandResponse : ValueObject {
        [ResourceSwaggerSchema("DocumentoAmministrativo")]
        public Documento DocumentoAmministrativo { get; init; }

        [ResourceSwaggerSchema("listaServiziApi")]
        public IEnumerable<Link> Links { get; set; }
    }
    public class AnnullamentoCommand : IRequest<AnnullamentoCommandResponse>
    {
        public string Id { get; set; }
        public string IdAutore { get; set; }
        public string Motivo { get; set; }
        public DateTime Data { get; set; }
    }
}
