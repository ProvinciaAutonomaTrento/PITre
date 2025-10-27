// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Pi3.App.AggregazioneDocumentale.WebApi.Binders;
using Pi3.App.AggregazioneDocumentale.WebApi.Resources;
using Pi3.Core.SeedWork;

namespace Pi3.App.AggregazioneDocumentale.WebApi.Application.Commands.Folders.Delete
{
    [ResourceSwaggerSchema(nameof(Documentation.CancellaCartellaCommandResponse))]
    public class CancellaCartellaCommandResponse : ValueObject
    {
        [ResourceSwaggerSchema(nameof(Documentation.listaServiziApi))]
        public IEnumerable<Link> Links { get; set; }
    }

    public class CancellaCartellaCommand : IRequest<CancellaCartellaCommandResponse>
    {
        public string Id { get; set; } = "";
        public string IdFolder { get; set; } = "";
    }
}
