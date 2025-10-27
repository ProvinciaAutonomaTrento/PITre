// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Pi3.App.AggregazioneDocumentale.WebApi.Binders;
using Pi3.App.AggregazioneDocumentale.WebApi.Resources;
using Pi3.Core.SeedWork;

namespace Pi3.App.AggregazioneDocumentale.WebApi.Application.Commands.Note.Add
{
    [ResourceSwaggerSchema(nameof(Documentation.TipoAccessoNotaEnum))]
    public enum TipoAccessoNotaEnum
    {
        Pubblica,
        RF,
        Ruolo,
        Personale
    }

    [ResourceSwaggerSchema(nameof(Documentation.TipiOggettoEnum))]
    public enum TipiOggettoEnum
    {
        Documento,
        Fascicolo
    }

    [ResourceSwaggerSchema(nameof(Documentation.AggiungiNotaCommandResponse))]
    public class AggiungiNotaCommandResponse : ValueObject
    {
        [ResourceSwaggerSchema(nameof(Documentation.IdNotaCreata))]
        public string Id { get; set; }

        [ResourceSwaggerSchema(nameof(Documentation.listaServiziApi))]
        public IEnumerable<Link> Links { get; set; }
    }

    [ResourceSwaggerSchema(nameof(Documentation.AutoreNota))]
    public class AutoreNota : ValueObject
    {
        [ResourceSwaggerSchema(nameof(Documentation.AutoreNota_IdUtente))]
        public string idUtente { get; init; }

        [ResourceSwaggerSchema(nameof(Documentation.AutoreNota_IdRuolo))]
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
