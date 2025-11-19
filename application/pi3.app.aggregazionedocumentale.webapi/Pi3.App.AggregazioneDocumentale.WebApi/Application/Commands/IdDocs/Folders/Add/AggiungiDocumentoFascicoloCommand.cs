// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Pi3.App.AggregazioneDocumentale.WebApi.Binders;
using Pi3.App.AggregazioneDocumentale.WebApi.Resources;
using Pi3.Core.SeedWork;

namespace Pi3.App.AggregazioneDocumentale.WebApi.Application.Commands.IdDocs.Folders.Add
{
    [ResourceSwaggerSchema(nameof(Documentation.AggiungiDocumentoFascicoloCommandResponse))]
    public class AggiungiDocumentoFascicoloCommandResponse : ValueObject
    {
        [ResourceSwaggerSchema(nameof(Documentation.listaServiziApi))]
        public IEnumerable<Link> Links { get; set; }
    }

    public class AggiungiDocumentoFascicoloCommand : IRequest<AggiungiDocumentoFascicoloCommandResponse>
    {
        public string Id { get; set; } = "";
        public string Identiticativo { get; set; } = "";
        public string IdFolder { get; set; } = "";
    }
}
