// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Pi3.App.DocumentoAmministrativo.WebApi.Binders;
using Pi3.Core.SeedWork;
using System.ComponentModel.DataAnnotations;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Queries.GetDocumentoAmministrativo
{
    [ResourceSwaggerSchema("GetDocumentoAmministrativoQueryResult")]
    public class GetDocumentoAmministrativoQueryResult : ValueObject
    {
        [ResourceSwaggerSchema("DocumentoAmministrativo")]
        public Documento DocumentoAmministrativo { get; init; }

        [ResourceSwaggerSchema("listaServiziApi")]
        public IEnumerable<Link> Links { get; set; }
    }

    public class GetDocumentoAmministrativoQuery : IRequest<GetDocumentoAmministrativoQueryResult>
    {
        [Required]
        public string Id { get; init; }

        public bool versioni { get; init; }

        public bool classificazioni { get; init; }

        public bool aggregazioni { get; init; }

        public bool permessi { get; init; }

        public bool profili { get; init; }

        public bool allegati { get; init; }

        public bool soggetti { get; init; }

        public bool note { get; init; }

        public bool keywords { get; init; }
    }
}
