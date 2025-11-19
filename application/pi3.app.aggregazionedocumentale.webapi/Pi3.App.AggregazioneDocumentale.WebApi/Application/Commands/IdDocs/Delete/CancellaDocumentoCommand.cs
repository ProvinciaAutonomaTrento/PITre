// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Pi3.App.AggregazioneDocumentale.WebApi.Binders;
using Pi3.App.AggregazioneDocumentale.WebApi.Resources;
using Pi3.Core.SeedWork;

namespace Pi3.App.AggregazioneDocumentale.WebApi.Application.Commands.IdDocs.Delete
{
    [ResourceSwaggerSchema(nameof(Documentation.CancellaDocumentoCommandResponse))]
    public class CancellaDocumentoCommandResponse : ValueObject
    {
        [ResourceSwaggerSchema(nameof(Documentation.listaServiziApi))]
        public IEnumerable<Link> Links { get; set; }
    }

    [ResourceSwaggerSchema(nameof(Documentation.CancellaDocumentoCommand))]
    public class CancellaDocumentoCommand : IRequest<CancellaDocumentoCommandResponse>
    {
        public string Id { get; set; }
        [ResourceSwaggerSchema(nameof(Documentation.CancellaDocumentoCommand_Identiticativo))]
        public string Identiticativo { get; set; }
    }
}
