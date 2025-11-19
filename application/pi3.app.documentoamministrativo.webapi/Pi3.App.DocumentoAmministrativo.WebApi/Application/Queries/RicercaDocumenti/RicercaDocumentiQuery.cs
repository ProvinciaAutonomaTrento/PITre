// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Pi3.App.DocumentoAmministrativo.WebApi.Binders;
using Pi3.Core.SeedWork;
using System.ComponentModel.DataAnnotations;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Queries.RicercaDocumenti;

public class Paginazione : ValueObject
{
    public int Ignora { get; set; }
    public int Prendi { get; set; }
}

public class Documento : ValueObject
{
    public string Id { get; init; } = null!;
    public DateTime CreationDate { get; init; }
    public string OggettoDelDocumento { get; init; } = null!;
    public DateTime? DataProtocollo { get; init; } = null;
    public long? NumeroProtocollo { get; init; } = null;
    public string? TipologiaFlusso { get; init; } = null;
    public string? Segnatura { get; init; } = null;
    public bool? Annullato { get; init; } = null;
}

[ResourceSwaggerSchema("RicercaDocumentiQueryResponse")]
public class RicercaDocumentiQueryResponse : ValueObject
{
    [ResourceSwaggerSchema("piuDati")]
    public bool PiuDati { get; set; }
    [ResourceSwaggerSchema("listaDocumentiRicerca")]
    public IList<Documento> Documenti { get; set; } = null!;
    [ResourceSwaggerSchema("listaServiziApi")]
    public IEnumerable<Link> Links { get; set; } = null!;
}

public enum TipiRicercheEnum
{
    NonProtocollati,
    Protocollati,
    Predisposti,
}

public enum TipologieFlussiRicercheEnum
{
    E,
    U,
    I,
}

public class RicercaDocumentiQuery: IRequest<RicercaDocumentiQueryResponse>
{
    [Required]
    [ResourceSwaggerSchema("tipoRicerca")]
    public TipiRicercheEnum? TipoRicerca { get; init; }

    [ResourceSwaggerSchema("tipologiaFlusso")]
    public TipologieFlussiRicercheEnum? TipologiaFlusso { get; init; }

    [ResourceSwaggerSchema("codiceRegistroRicerca")]
    public string? CodiceRegistro { get; init; }

    [Required]
    [ResourceSwaggerSchema("annoRicerca")]

    public int? Anno { get; init; }

    public int? NumeroProtocollo { get; init; } = null;

    public int? NumeroProtocolloFinale { get; init; } = null;

    [Required]
    public Paginazione? Paginazione { get; init; }
}