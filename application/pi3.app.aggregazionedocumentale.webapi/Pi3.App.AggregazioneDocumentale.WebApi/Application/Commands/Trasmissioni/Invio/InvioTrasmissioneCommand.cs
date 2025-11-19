// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Pi3.App.AggregazioneDocumentale.WebApi.Binders;
using Pi3.App.AggregazioneDocumentale.WebApi.Resources;
using Pi3.Core.SeedWork;

namespace Pi3.App.AggregazioneDocumentale.WebApi.Application.Commands.Trasmissioni.Invio
{
    [ResourceSwaggerSchema(nameof(Documentation.InvioTrasmissioneCommandResponse))]
    public class InvioTrasmissioneCommandResponse: ValueObject {
        [ResourceSwaggerSchema(nameof(Documentation.listaServiziApi))]
        public IEnumerable<Link> Links { get; set; }
    }

    public class InvioTrasmissioneCommand: IRequest<InvioTrasmissioneCommandResponse>
    {
        public string Id { get; set; }
        public DateTime DataInvio { get; set; }
    }
}
