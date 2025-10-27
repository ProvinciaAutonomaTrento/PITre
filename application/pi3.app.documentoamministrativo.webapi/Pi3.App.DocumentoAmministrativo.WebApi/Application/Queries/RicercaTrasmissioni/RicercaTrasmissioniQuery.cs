// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Pi3.App.DocumentoAmministrativo.WebApi.Binders;
using Pi3.Core.SeedWork;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Queries.RicercaTrasmissioni
{
    public class Autore : ValueObject
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
    }

    [ResourceSwaggerSchema("RicercaTrasmissioniQueryResponse")]
    public class RicercaTrasmissioniQueryResponse : ValueObject
    {
        [ResourceSwaggerSchema("piuDati")]
        public bool PiuDati { get; set; }

        [ResourceSwaggerSchema("Trasmissioni")]
        public IEnumerable<Trasmissione> Trasmissioni { get; init; }

        [ResourceSwaggerSchema("listaServiziApi")]
        public IEnumerable<Link> Links { get; set; }
    }

    public class RicercaTrasmissioniQuery : IRequest<RicercaTrasmissioniQueryResponse>
    {
        public int Ignora { get; set; }

        public int Prendi { get; set; }

        public string IdDocumento { get; set; } = null!;
    }
}
