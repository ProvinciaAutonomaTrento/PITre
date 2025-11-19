// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later

using MediatR;
using Pi3.App.DocumentoAmministrativo.WebApi.Binders;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Trasmissioni.Add;

[ResourceSwaggerSchema("AggiungiTrasmissioneCommandResponse")]
public class AggiungiTrasmissioneCommandResponse {
    [ResourceSwaggerSchema("IdTrasmissioneCreata")]
    public string Id { get; set; }

    [ResourceSwaggerSchema("listaServiziApi")]
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

