// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Pi3.App.AggregazioneDocumentale.WebApi.Binders;
using Pi3.App.AggregazioneDocumentale.WebApi.Resources;
using Pi3.Core.SeedWork;

namespace Pi3.App.AggregazioneDocumentale.WebApi.Application.Commands.IdDocs.Add
{
    [ResourceSwaggerSchema(nameof(Documentation.AggiungiDocumentoCommandResponse))]
    public class AggiungiDocumentoCommandResponse : ValueObject
    {
        [ResourceSwaggerSchema(nameof(Documentation.listaServiziApi))]
        public IEnumerable<Link> Links { get; set; }

    }
    public class AggiungiDocumentoCommand : IRequest<AggiungiDocumentoCommandResponse>
    {
        public string Id { get; set; } = "";
        public string Identiticativo { get; set; } = "";
    }
}
