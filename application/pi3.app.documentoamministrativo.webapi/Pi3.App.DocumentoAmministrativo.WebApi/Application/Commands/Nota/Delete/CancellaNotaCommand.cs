// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Pi3.App.DocumentoAmministrativo.WebApi.Binders;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Note.Delete;

[ResourceSwaggerSchema("CancellaNotaCommandResponse")]
public class CancellaNotaCommandResponse
{
    [ResourceSwaggerSchema("listaServiziApi")]
    public IEnumerable<Link> Links { get; set; }
}

public class CancellaNotaCommand : IRequest<CancellaNotaCommandResponse>
{
    public string Id { get; set; }
    public string IdNota { get; set; }
}
