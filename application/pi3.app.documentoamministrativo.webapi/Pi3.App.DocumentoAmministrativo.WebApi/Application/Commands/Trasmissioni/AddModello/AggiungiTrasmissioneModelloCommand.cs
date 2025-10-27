// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2

using MediatR;
using Pi3.App.DocumentoAmministrativo.WebApi.Binders;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Trasmissioni.AddModello;

[ResourceSwaggerSchema("AggiungiTrasmissioneModelloCommandResponse")]
public class AggiungiTrasmissioneModelloCommandResponse
{
    [ResourceSwaggerSchema("IdNotaCreata")]
    public string Id { get; set; }

    [ResourceSwaggerSchema("listaServiziApi")]
    public IEnumerable<Link> Links { get; set; }
}

public class AggiungiTrasmissioneModelloCommand : IRequest<AggiungiTrasmissioneModelloCommandResponse>
{
    public string? IdAggregato { get; set; }
    public string? Modello { get; set; }
}
