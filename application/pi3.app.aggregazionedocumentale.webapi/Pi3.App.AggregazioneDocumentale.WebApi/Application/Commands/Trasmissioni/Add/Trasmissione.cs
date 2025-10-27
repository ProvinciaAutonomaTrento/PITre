// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Pi3.App.AggregazioneDocumentale.WebApi.Binders;
using Pi3.App.AggregazioneDocumentale.WebApi.Resources;

namespace Pi3.App.AggregazioneDocumentale.WebApi.Application.Commands.Trasmissioni.Add
{
    [ResourceSwaggerSchema(nameof(Documentation.Trasmissione_Head))]
    public class Trasmissione
    {
        [ResourceSwaggerSchema(nameof(Documentation.Trasmissione_append))]
        public bool append { get; set; }
        [ResourceSwaggerSchema(nameof(Documentation.Trasmissione_note))]
        public string? noteTrasmissione { get; set; }
        [ResourceSwaggerSchema(nameof(Documentation.Trasmissione_gruppi))]
        public IEnumerable<GruppoDestinatario>? gruppiDestinatari { get; set; }
        [ResourceSwaggerSchema(nameof(Documentation.Trasmissione_utenti))]
        public IEnumerable<UtenteDestinatario>? utentiDestinatari { get; set; }
    }
}
