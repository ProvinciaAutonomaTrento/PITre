// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Pi3.App.DocumentoAmministrativo.WebApi.Binders;
using Pi3.Core.SeedWork;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Trasmissioni.Invio;

[ResourceSwaggerSchema("InvioTrasmissioneCommandResponse")]
public class InvioTrasmissioneCommandResponse: ValueObject {

    [ResourceSwaggerSchema("listaServiziApi")]
    public IEnumerable<Link> Links { get; set; }
}

public class InvioTrasmissioneCommand: IRequest<InvioTrasmissioneCommandResponse>
{
    public string Id { get; set; }
    public DateTime DataInvio { get; set; }
}
