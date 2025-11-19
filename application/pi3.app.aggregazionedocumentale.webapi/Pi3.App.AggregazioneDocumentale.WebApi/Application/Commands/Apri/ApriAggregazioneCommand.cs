// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Pi3.App.AggregazioneDocumentale.WebApi.Binders;
using Pi3.App.AggregazioneDocumentale.WebApi.Resources;

namespace Pi3.App.AggregazioneDocumentale.WebApi.Application.Commands.Apri;


[ResourceSwaggerSchema(nameof(Documentation.ApriAggregazioneCommandResponse))]
public class ApriAggregazioneCommandResponse
{
    [ResourceSwaggerSchema(nameof(Documentation.listaServiziApi))]
    public IEnumerable<Link> Links { get; set; }
}

public class ApriAggregazioneCommand: IRequest<ApriAggregazioneCommandResponse>
{
    public string Id { get; set; }
}
