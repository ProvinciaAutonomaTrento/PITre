// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Pi3.Core.SeedWork;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Queries;
using Pi3.App.DocumentoAmministrativo.WebApi.Binders;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Protocollo.Emergenza;

[ResourceSwaggerSchema("ProtocolloEmergenza_Head")]
public class ProtocolloEmergenza : ValueObject
{
    [ResourceSwaggerSchema("Segnatura")]
    public string? Segnatura { get; init; }
    [ResourceSwaggerSchema("DataProtocolloEmergenza")]
    public DateTime? Data { get; init; }

    public string Cognome { get; init; }
    public string Nome { get; init; }
}


[ResourceSwaggerSchema("ProtocolloEmergenzaCommandResponse")]
public class ProtocolloEmergenzaCommandResponse : ValueObject
{
    [ResourceSwaggerSchema("DocumentoAmministrativo")]
    public Documento DocumentoAmministrativo { get; init; }

    [ResourceSwaggerSchema("listaServiziApi")]
    public IEnumerable<Link> Links { get; set; }
}

public class ProtocolloEmergenzaCommand : IRequest<ProtocolloEmergenzaCommandResponse>
{
    public string Id { get; init; }
    public ProtocolloEmergenza Protocollo { get; init; }
}
