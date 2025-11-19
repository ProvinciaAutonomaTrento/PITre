// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Queries;
using Pi3.App.DocumentoAmministrativo.WebApi.Binders;
using Pi3.Core.SeedWork;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Documenti.DelCollegato;

[ResourceSwaggerSchema("DelDocumentoCollegatoCommandResponse")]
public class DelDocumentoCollegatoCommandResponse : ValueObject
{
    [ResourceSwaggerSchema("DocumentoAmministrativo")]
    public Documento DocumentoAmministrativo { get; init; }

    [ResourceSwaggerSchema("listaServiziApi")]
    public IEnumerable<Link> Links { get; set; }

}
public class DelDocumentoCollegatoCommand : IRequest<DelDocumentoCollegatoCommandResponse>
{
    public string Id { get; set; }
    public string IdCollegato { get; set; }
}
