// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2

using MediatR;
using Pi3.App.AggregazioneDocumentale.WebApi.Binders;
using Pi3.App.AggregazioneDocumentale.WebApi.Resources;

namespace Pi3.App.AggregazioneDocumentale.WebApi.Application.Commands.Trasmissioni.Add
{
    [ResourceSwaggerSchema(nameof(Documentation.AggiungiTrasmissioneCommandResponse))]
    public class AggiungiTrasmissioneCommandResponse {
        [ResourceSwaggerSchema(nameof(Documentation.IdTrasmissioneCreata))]
        public string Id { get; set; }
        [ResourceSwaggerSchema(nameof(Documentation.listaServiziApi))]
        public IEnumerable<Link> Links { get; set; }
    }

    public class AggiungiTrasmissioneCommand: IRequest<AggiungiTrasmissioneCommandResponse>
    {
        public string IdAggregato { get; set; }
        public string? NoteTrasmissione { get; set; }
        public IEnumerable<GruppoDestinatario>? GruppiDestinatari { get; init; }
        public IEnumerable<UtenteDestinatario>? UtentiDestinatari { get; init; }

        public bool? Invio { get; set; }
        public string ? UpdatedId { get; set; }
        public bool? Append { get; set; }

    }
}
