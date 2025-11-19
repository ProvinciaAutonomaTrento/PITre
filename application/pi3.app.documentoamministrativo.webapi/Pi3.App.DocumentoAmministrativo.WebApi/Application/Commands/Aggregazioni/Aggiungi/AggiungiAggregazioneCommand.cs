// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Queries;
using Pi3.App.DocumentoAmministrativo.WebApi.Binders;
using Pi3.Core.SeedWork;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Aggregazioni.Aggiungi;

[ResourceSwaggerSchema("AggiungiAggregazioneCommandResponse")]
public class AggiungiAggregazioneCommandResponse : ValueObject
{
    [ResourceSwaggerSchema("DocumentoAmministrativo")]
    public Documento DocumentoAmministrativo { get; init; }

    [ResourceSwaggerSchema("listaServiziApi")]
    public IEnumerable<Link> Links { get; set; }

}

public class AggiungiAggregazioneCommand : IRequest<AggiungiAggregazioneCommandResponse>
{
    public string Id { get; init; }
    public Aggregazione Aggregazione { get; init; }
}
