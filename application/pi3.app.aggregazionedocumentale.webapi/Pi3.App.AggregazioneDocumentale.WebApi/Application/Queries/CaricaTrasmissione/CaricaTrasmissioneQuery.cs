// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Pi3.App.AggregazioneDocumentale.WebApi.Binders;
using Pi3.App.AggregazioneDocumentale.WebApi.Resources;
using Pi3.Core.SeedWork;

namespace Pi3.App.AggregazioneDocumentale.WebApi.Application.Queries.CaricaTrasmissione;

[ResourceSwaggerSchema(nameof(Documentation.Autore_Trasmissione))]
public class Autore : ValueObject
{
    [ResourceSwaggerSchema(nameof(Documentation.Autore_UserId))]
    public string UserId { get; init; }
    [ResourceSwaggerSchema(nameof(Documentation.Autore_Nome))]
    public string Nome { get; init; }
    [ResourceSwaggerSchema(nameof(Documentation.Autore_Cognome))]
    public string Cognome { get; init; }
    public string UserIdDelegato { get; init; }
    public string NomeDelegato { get; init; }
    public string CognomeDelegato { get; init; }
    [ResourceSwaggerSchema(nameof(Documentation.Autore_CodiceGruppo))]
    public string CodiceGruppo { get; init; }
    [ResourceSwaggerSchema(nameof(Documentation.Autore_DescrizioneGruppo))]
    public string DescrizioneGruppo { get; init; }
}

[ResourceSwaggerSchema(nameof(Documentation.UtenteNotificato))]
public class UtenteNotificato : ValueObject
{
    [ResourceSwaggerSchema(nameof(Documentation.UtenteDestinatario))]
    public UtenteDestinatario UtenteDestinatario { get; set; }

    [ResourceSwaggerSchema(nameof(Documentation.DataVista))]
    public DateTime? DataVista { get; set; }

    [ResourceSwaggerSchema(nameof(Documentation.DataAccettazione))]
    public DateTime? DataAccettazione { get; set; }

    [ResourceSwaggerSchema(nameof(Documentation.NoteAccettazione))]
    public TextValue? NoteAccettazione { get; set; }

    [ResourceSwaggerSchema(nameof(Documentation.DataRifiuto))]
    public DateTime? DataRifiuto { get; set; }

    [ResourceSwaggerSchema(nameof(Documentation.NoteRifiuto))]
    public TextValue? NoteRifiuto { get; set; }

    public DateTime? DataRimozioneCentroNotifiche { get; set; }

    public UtenteDestinatario? UtenteDelegato { get; set; }
}

[ResourceSwaggerSchema(nameof(Documentation.UtenteDestinatario))]
public class UtenteDestinatario : ValueObject
{
    [ResourceSwaggerSchema(nameof(Documentation.UtenteDestinatario_UserId))]
    public string UserId { get; set; }
    [ResourceSwaggerSchema(nameof(Documentation.UtenteDestinatario_Nome))]
    public string Nome { get; set; }
    [ResourceSwaggerSchema(nameof(Documentation.UtenteDestinatario_Cognome))]
    public string Cognome { get; set; }
    [ResourceSwaggerSchema(nameof(Documentation.RagioneTrasmissione))]
    public string RagioneTrasmissione { get; set; }
    [ResourceSwaggerSchema(nameof(Documentation.Note_Trasmissione))]
    public string? NoteTrasmissione { get; set; }
    [ResourceSwaggerSchema(nameof(Documentation.Trasmissione_GiorniScadenza))]
    public string? GiorniScadenza { get; set; }
}

[ResourceSwaggerSchema(nameof(Documentation.GruppoDestinatario))]
public class GruppoDestinatario : ValueObject
{
    [ResourceSwaggerSchema(nameof(Documentation.descrizioneGruppo))]
    public string DescrizioneGruppo { get; set; }
    [ResourceSwaggerSchema(nameof(Documentation.RagioneTrasmissione))]
    public string RagioneTrasmissione { get; set; }
    [ResourceSwaggerSchema(nameof(Documentation.TipoGruppo))]
    public string? Tipo { get; set; }
    [ResourceSwaggerSchema(nameof(Documentation.Note_Trasmissione))]
    public string? NoteTrasmissione { get; set; }
    [ResourceSwaggerSchema(nameof(Documentation.Note_Scadenza))]
    public string? DataScadenza { get; set; }
    [ResourceSwaggerSchema(nameof(Documentation.Utenti_Notificati))]
    public IEnumerable<UtenteNotificato>? UtentiNotificati { get; set; }
}


[ResourceSwaggerSchema(nameof(Documentation.Trasmissione_Full))]
public class Trasmissione : ValueObject {
    [ResourceSwaggerSchema(nameof(Documentation.Trasmissione_Id))]
    public string? Id { get; init; }
    [ResourceSwaggerSchema(nameof(Documentation.Trasmissione_DataInvio))]
    public string DataInvio { get; init; }
    [ResourceSwaggerSchema(nameof(Documentation.Trasmissione_note))]
    public string? NoteGenerali { get; init; }
    [ResourceSwaggerSchema(nameof(Documentation.Trasmissione_Autore))]
    public Autore? Autore { get; init; }
    [ResourceSwaggerSchema(nameof(Documentation.Trasmissione_gruppi))]
    public IList<GruppoDestinatario>? GruppiDestinatari { get; set; }
    [ResourceSwaggerSchema(nameof(Documentation.Trasmissione_utenti))]
    public IList<UtenteDestinatario>? UtentiDestinatari { get; set; }

}

[ResourceSwaggerSchema(nameof(Documentation.CaricaTrasmissioneQueryResponse))]
public class CaricaTrasmissioneQueryResponse: ValueObject
{
    [ResourceSwaggerSchema(nameof(Documentation.Trasmissione_Full))]
    public Trasmissione Trasmissione { get; set; }
    [ResourceSwaggerSchema(nameof(Documentation.listaServiziApi))]
    public IEnumerable<Link> Links { get; set; }
}

public class CaricaTrasmissioneQuery: IRequest<CaricaTrasmissioneQueryResponse>
{
    public  string Id { get; set; }
}
