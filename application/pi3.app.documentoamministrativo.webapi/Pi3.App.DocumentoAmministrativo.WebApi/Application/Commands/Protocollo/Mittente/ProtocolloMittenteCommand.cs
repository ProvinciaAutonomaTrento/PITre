// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Pi3.Core.SeedWork;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Queries;
using Pi3.App.DocumentoAmministrativo.WebApi.Binders;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Protocollo.Mittente;

[ResourceSwaggerSchema("ProtocolloMittente_Head")]
public class ProtocolloMittente : ValueObject
{
    [ResourceSwaggerSchema("Segnatura")]
    public string? Segnatura { get; init; }
    [ResourceSwaggerSchema("DataProtocollazioneMittente")]

    public DateTime? Data { get; init; }

    [ResourceSwaggerSchema("DataArrivoDocumento")]
    public DateTime? DataArrivo { get; init; }
}

[ResourceSwaggerSchema("ProtocolloMittenteCommandResponse")]
public class ProtocolloMittenteCommandResponse : ValueObject
{
    [ResourceSwaggerSchema("DocumentoAmministrativo")]
    public Documento DocumentoAmministrativo { get; init; }


    [ResourceSwaggerSchema("listaServiziApi")]
    public IEnumerable<Link> Links { get; set; }
}

public class ProtocolloMittenteCommand : IRequest<ProtocolloMittenteCommandResponse>
{
    public string Id { get; init; }
    public ProtocolloMittente Protocollo { get; init; }
}
