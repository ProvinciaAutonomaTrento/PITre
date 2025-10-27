// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Queries;
using Pi3.App.DocumentoAmministrativo.WebApi.Binders;
using Pi3.Core.SeedWork;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Documenti.AddCollegato;

[ResourceSwaggerSchema("AddDocumentoCollegatoCommandResponse")]
public class AddDocumentoCollegatoCommandResponse : ValueObject
{
    [ResourceSwaggerSchema("DocumentoAmministrativo")]
    public Documento DocumentoAmministrativo { get; init; }

    [ResourceSwaggerSchema("DocumentoAmministrativo")]
    public IEnumerable<Link> Links { get; set; }
}
public class AddDocumentoCollegatoCommand : IRequest<AddDocumentoCollegatoCommandResponse>
{
    public string Id { get; set; }
    public string IdDocumento { get; set; }
    public bool IsParent { get; set; }
}
