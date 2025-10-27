// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Pi3.App.DocumentoAmministrativo.WebApi.Binders;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Trasmissioni.Add;

[ResourceSwaggerSchema("Trasmissione_Head")]
public class Trasmissione
{
    [ResourceSwaggerSchema("Trasmissione_Append")]
    public bool append { get; set; }

    [ResourceSwaggerSchema("Trasmissione_NoteTrasmisssione")]
    public string? noteTrasmissione { get; set; }

    [ResourceSwaggerSchema("GruppiDestinatari")]
    public IEnumerable<GruppoDestinatario>? gruppiDestinatari { get; set; }

    [ResourceSwaggerSchema("UtentiDestinatari")]
    public IEnumerable<UtenteDestinatario>? utentiDestinatari { get; set; }
}
