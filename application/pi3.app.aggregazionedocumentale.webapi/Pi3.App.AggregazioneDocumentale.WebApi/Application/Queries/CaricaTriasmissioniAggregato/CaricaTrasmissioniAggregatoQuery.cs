// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Pi3.App.AggregazioneDocumentale.WebApi.Binders;
using Pi3.App.AggregazioneDocumentale.WebApi.Resources;
using Pi3.Core.SeedWork;

namespace Pi3.App.AggregazioneDocumentale.WebApi.Application.Queries.CaricaTriasmissioniAggregato
{
    [ResourceSwaggerSchema(nameof(Documentation.UtenteDestinatario))]
    public class UtenteDestinatario : ValueObject
    {
        [ResourceSwaggerSchema(nameof(Documentation.UtenteDestinatario_UserId))]
        public string UserId { get; init; }
        [ResourceSwaggerSchema(nameof(Documentation.UtenteDestinatario_Nome))]
        public string Nome { get; init; }
        [ResourceSwaggerSchema(nameof(Documentation.UtenteDestinatario_Cognome))]
        public string Cognome { get; init; }
        [ResourceSwaggerSchema(nameof(Documentation.RagioneTrasmissione))]
        public string RagioneTrasmissione { get; init; }
        [ResourceSwaggerSchema(nameof(Documentation.Note_Trasmissione))]
        public string NoteTrasmissione { get; init; }
        [ResourceSwaggerSchema(nameof(Documentation.Note_Scadenza))]
        public DateTime DataScadenza { get; init; }
    }

    [ResourceSwaggerSchema(nameof(Documentation.UtenteNotificato))]
    public class UtenteNotificato : ValueObject
    {
        [ResourceSwaggerSchema(nameof(Documentation.Notificato_UserId))]
        public string UserId { get; init; }
        [ResourceSwaggerSchema(nameof(Documentation.Notificato_Nome))]
        public string Nome { get; init; }
        [ResourceSwaggerSchema(nameof(Documentation.Notificato_Cognome))]
        public string Cognome { get; init; }
    }

    [ResourceSwaggerSchema(nameof(Documentation.GruppoDestinatario))]
    public class GruppoDestinatario : ValueObject
    {
        [ResourceSwaggerSchema(nameof(Documentation.codiceGruppo))]
        public string CodiceGruppo { get; init; }
        [ResourceSwaggerSchema(nameof(Documentation.descrizioneGruppo))]
        public string DescrizioneGruppo { get; init; }
        [ResourceSwaggerSchema(nameof(Documentation.RagioneTrasmissione))]
        public string RagioneTrasmissione { get; init; }
        [ResourceSwaggerSchema(nameof(Documentation.TipoGruppo))]
        public string Tipo { get; init; }
        [ResourceSwaggerSchema(nameof(Documentation.Note_Trasmissione))]
        public string NoteTrasmissione { get; init; }
        [ResourceSwaggerSchema(nameof(Documentation.Note_Scadenza))]
        public string DataScadenza { get; init; }

        [ResourceSwaggerSchema(nameof(Documentation.Utenti_Notificati))]
        public IEnumerable<UtenteNotificato> UtentiNotificati { get; init; }
    }

    [ResourceSwaggerSchema(nameof(Documentation.Autore_Head))]
    public class Autore: ValueObject
    {
        [ResourceSwaggerSchema(nameof(Documentation.Autore_UserId))]
        public string UserId { get; set; }
        [ResourceSwaggerSchema(nameof(Documentation.Autore_Nome))]
        public string Nome { get; set; }
        [ResourceSwaggerSchema(nameof(Documentation.Autore_Cognome))]
        public string Cognome { get; set; }
        public string UserIdDelegato { get; set; }
        public string NomeDelegato { get; set; }
        public string CognomeDelegato { get; set; }
        [ResourceSwaggerSchema(nameof(Documentation.Autore_CodiceGruppo))]
        public string CodiceGruppo { get; set; }
        [ResourceSwaggerSchema(nameof(Documentation.Autore_DescrizioneGruppo))]
        public string DescrizioneGruppo { get; set; }
    }

    [ResourceSwaggerSchema(nameof(Documentation.Trasmissione))]
    public class Trasmissione : ValueObject
    {
        [ResourceSwaggerSchema(nameof(Documentation.Trasmissione_Id))]
        public string Id { get; init; }
        [ResourceSwaggerSchema(nameof(Documentation.Trasmissione_Autore))]
        public Autore Autore { get; init; }
        [ResourceSwaggerSchema(nameof(Documentation.Trasmissione_Autore))]
        public DateTime? DataInvio { get; set; }
        [ResourceSwaggerSchema(nameof(Documentation.Trasmissione_note))]
        public string NoteGenerali { get; set; }
        [ResourceSwaggerSchema(nameof(Documentation.Trasmissione_gruppi))]
        public IEnumerable<GruppoDestinatario> GruppiDestinatari { get; set; }
        [ResourceSwaggerSchema(nameof(Documentation.Trasmissione_utenti))]
        public IEnumerable<UtenteDestinatario> UtentiDestinatari { get; set; }
    }

    [ResourceSwaggerSchema(nameof(Documentation.CaricaTrasmissioniAggregatoQueryResponse))]
    public class CaricaTrasmissioniAggregatoQueryResponse: ValueObject
    {
        [ResourceSwaggerSchema(nameof(Documentation.Trasmissioni))]
        public IEnumerable<Trasmissione> Trasmissioni { get; init; }
        [ResourceSwaggerSchema(nameof(Documentation.listaServiziApi))]
        public IEnumerable<Link> Links { get; set; }
    }

    public class CaricaTrasmissioniAggregatoQuery: IRequest<CaricaTrasmissioniAggregatoQueryResponse>
    {
        public string Id { get; set; }
    }
}
