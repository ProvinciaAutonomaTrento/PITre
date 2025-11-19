// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Pi3.App.DocumentoAmministrativo.WebApi.Binders;
using Pi3.Core.SeedWork;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Trasmissioni.Rifiuto;

[ResourceSwaggerSchema("VistoTrasmissioneCommandResponse")]
public class VistoTrasmissioneCommandResponse : ValueObject
{
    [ResourceSwaggerSchema("listaServiziApi")]
    public IEnumerable<Link> Links { get; set; }
}

public class VistoTrasmissioneCommand : IRequest<VistoTrasmissioneCommandResponse>
{
    public string Id  { get; set; }
}
