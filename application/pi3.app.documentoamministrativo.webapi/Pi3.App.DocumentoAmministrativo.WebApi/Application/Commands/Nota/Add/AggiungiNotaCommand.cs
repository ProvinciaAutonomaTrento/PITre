// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Pi3.App.DocumentoAmministrativo.WebApi.Binders;
using Pi3.Core.SeedWork;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Note.Add
{
    public enum TipoAccessoNotaEnum
    {
        Pubblica,
        RF,
        Ruolo,
        Personale
    }

    public enum TipiOggettoEnum
    {
        Documento,
        Fascicolo
    }

    [ResourceSwaggerSchema("AggiungiNotaCommandResponse")]
    public class AggiungiNotaCommandResponse : ValueObject
    {
        [ResourceSwaggerSchema("IdNotaCreata")]
        public string Id { get; set; }

        [ResourceSwaggerSchema("listaServiziApi")]
        public IEnumerable<Link> Links { get; set; }
    }

    [ResourceSwaggerSchema("AutoreNota")]
    public class AutoreNota : ValueObject
    {
        
        [ResourceSwaggerSchema("IdUtenteNota")]
        public string idUtente { get; init; }

        [ResourceSwaggerSchema("idRuoloNota")]
        public string idRuolo { get; init; }

        public string? idUtenteDelegato { get; init; }
    }

    public class AggiungiNotaCommand : IRequest<AggiungiNotaCommandResponse>
    {
        public string? id { get; set; }
        public string nome { get; set; } = "";
        public string description { get; set; } = "";
        public AutoreNota? autore { get; set; }
        public string? idOggetto { get; set; } = "";
        public TipiOggettoEnum? tipoOggetto { get; set; }
        public TipoAccessoNotaEnum tipoAccesso { get; set; }
        public string? idAccessoRF { get; set; }
    }
}
