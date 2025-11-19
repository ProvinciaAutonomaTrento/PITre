// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Pi3.App.DocumentoAmministrativo.WebApi.Binders;
using Pi3.Core.SeedWork;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Queries.GetTrasmissione
{
    public class UtenteDestinatario : ValueObject
    {
        [ResourceSwaggerSchema("UserIdDestinatario")]
        public string UserId { get; init; }

        public string Nome { get; init; }

        public string Cognome { get; init; }

        [ResourceSwaggerSchema("RagioneTrasmissione")]
        public string RagioneTrasmissione { get; init; }

        [ResourceSwaggerSchema("NoteTrasmissione")]
        public string NoteTrasmissione { get; init; }

        [ResourceSwaggerSchema("DataScadenza")]
        public DateTime? DataScadenza { get; init; }
    }

    public class UtenteNotificato : ValueObject
    {
        [ResourceSwaggerSchema("UtenteNotificato_UserId")]
        public string UserId { get; init; }
        [ResourceSwaggerSchema("UtenteNotificato_Nome")]
        public string Nome { get; init; }
        [ResourceSwaggerSchema("UtenteNotificato_Cognome")]
        public string Cognome { get; init; }
    }

    public class GruppoDestinatario : ValueObject
    {
        [ResourceSwaggerSchema("CodiceGruppoDestinatario")]
        public string CodiceGruppo { get; init; }

        public string DescrizioneGruppo { get; init; }

        [ResourceSwaggerSchema("RagioneTrasmissione")]
        public string RagioneTrasmissione { get; init; }

        [ResourceSwaggerSchema("TipoTrasmissione")]
        public string Tipo { get; init; }

        [ResourceSwaggerSchema("NoteTrasmissione")]
        public string NoteTrasmissione { get; init; }

        [ResourceSwaggerSchema("DataScadenza")]
        public string DataScadenza { get; init; }

        [ResourceSwaggerSchema("UtentiNotificati")]
        public IEnumerable<UtenteNotificato> UtentiNotificati { get; init; }
    }

    public class Autore: ValueObject
    {
        [ResourceSwaggerSchema("Autore_UserId")]
        public string UserId { get; set; }
        [ResourceSwaggerSchema("Autore_Nome")]
        public string Nome { get; set; }
        [ResourceSwaggerSchema("Autore_Cognome")]
        public string Cognome { get; set; }
        public string UserIdDelegato { get; set; }
        public string NomeDelegato { get; set; }
        public string CognomeDelegato { get; set; }
        [ResourceSwaggerSchema("Autore_CodiceGruppo")]
        public string CodiceGruppo { get; set; }
        public string DescrizioneGruppo { get; set; }
    }

    public class Trasmissione : ValueObject
    {
        [ResourceSwaggerSchema("idTrasmissione")]
        public string Id { get; init; }
        [ResourceSwaggerSchema("InfoAutore")]
        public Autore Autore { get; init; }

        [ResourceSwaggerSchema("NoteGenerali")]
        public DateTime? DataInvio { get; set; }

        [ResourceSwaggerSchema("NoteGenerali")]
        public string NoteGenerali { get; set; }

        [ResourceSwaggerSchema("GruppiDestinatari")]
        public IEnumerable<GruppoDestinatario> GruppiDestinatari { get; set; }

        [ResourceSwaggerSchema("UtentiDestinatari")]
        public IEnumerable<UtenteDestinatario> UtentiDestinatari { get; set; }
    }

    [ResourceSwaggerSchema("TrasmissioniDocumentoQueryResponse")]
    public class GetTrasmissioneQueryResponse : ValueObject
    {
        [ResourceSwaggerSchema("Trasmissione")]
        public Trasmissione Trasmissione { get; init; }

        [ResourceSwaggerSchema("listaServiziApi")]
        public IEnumerable<Link> Links { get; set; }
    }

    public class GetTrasmissioneQuery : IRequest<GetTrasmissioneQueryResponse>
    {
        public string Id { get; set; }
    }
}
