// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Pi3.Core.SeedWork;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Queries;
using Pi3.App.DocumentoAmministrativo.WebApi.Binders;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Oggetto;

[ResourceSwaggerSchema("OggettoDocumentoCommandResponse")]
public class OggettoDocumentoCommandResponse : ValueObject
{
    [ResourceSwaggerSchema("DocumentoAmministrativo")]
    public Documento DocumentoAmministrativo { get; init; }

    [ResourceSwaggerSchema("listaServiziApi")]
    public IEnumerable<Link> Links { get; set; }
}

public class OggettoDocumentoCommand : IRequest<OggettoDocumentoCommandResponse>
{
    public string Id { get; init; }
    public string Descrizione { get; init; }
}
