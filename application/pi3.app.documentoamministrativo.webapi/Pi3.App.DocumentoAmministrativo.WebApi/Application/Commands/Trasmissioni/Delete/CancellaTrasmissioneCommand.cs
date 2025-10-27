// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Pi3.App.DocumentoAmministrativo.WebApi.Binders;
using Pi3.Core.SeedWork;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Trasmissioni.Delete;


[ResourceSwaggerSchema("CancellaTrasmissioneCommandResponse")]
public class CancellaTrasmissioneCommandResponse : ValueObject
{

    [ResourceSwaggerSchema("listaServiziApi")]
    public IEnumerable<Link> Links { get; set; }
}

public class CancellaTrasmissioneCommand: IRequest<CancellaTrasmissioneCommandResponse>
{
    public string Id { get; set; }
}
