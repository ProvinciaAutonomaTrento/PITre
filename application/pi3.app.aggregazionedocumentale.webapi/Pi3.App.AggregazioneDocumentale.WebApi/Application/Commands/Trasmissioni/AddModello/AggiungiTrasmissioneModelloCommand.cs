// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2

using MediatR;
using Pi3.App.AggregazioneDocumentale.WebApi.Binders;
using Pi3.App.AggregazioneDocumentale.WebApi.Resources;

namespace Pi3.App.AggregazioneDocumentale.WebApi.Application.Commands.Trasmissioni.AddModello;

[ResourceSwaggerSchema(nameof(Documentation.AggiungiTrasmissioneModelloCommandResponse))]
public class AggiungiTrasmissioneModelloCommandResponse
{
    [ResourceSwaggerSchema(nameof(Documentation.IdTrasmissioneCreata))]
    public string Id { get; set; }
    [ResourceSwaggerSchema(nameof(Documentation.listaServiziApi))]
    public IEnumerable<Link> Links { get; set; }
}

public class AggiungiTrasmissioneModelloCommand : IRequest<AggiungiTrasmissioneModelloCommandResponse>
{
    public string? IdAggregato { get; set; }
    public string? Modello { get; set; }
}
