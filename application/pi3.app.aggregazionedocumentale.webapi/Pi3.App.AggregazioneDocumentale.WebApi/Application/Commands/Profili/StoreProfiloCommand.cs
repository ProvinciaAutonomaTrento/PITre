// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Pi3.App.AggregazioneDocumentale.WebApi.Binders;
using Pi3.App.AggregazioneDocumentale.WebApi.Resources;
using Pi3.Core.SeedWork;

namespace Pi3.App.AggregazioneDocumentale.WebApi.Application.Commands.Profili
{
    [ResourceSwaggerSchema(nameof(Documentation.StoreProfiloCommandResponse))]
    public class StoreProfiloCommandResponse : ValueObject {
        public string Id { get; set; }
        [ResourceSwaggerSchema(nameof(Documentation.listaServiziApi))]
        public IEnumerable<Link> Links { get; set; }
    }

    [ResourceSwaggerSchema(nameof(Documentation.TipoCampoEnum))]
    public enum TipoCampoEnum
    {
        CampoDiTesto,
        CasellaSelezione,
        CasellaSelezioneEsclusiva,
        MenuTendina,
        Corrispondente,
        Contatore,
        Data,
        Orario,
        OrarioSecondi
    }


    [ResourceSwaggerSchema(nameof(Documentation.CampoProfilo))]
    public class Campo
    {
        [ResourceSwaggerSchema(nameof(Documentation.NomeCampoProfilo))]
        public string Nome { get; set; }
        [ResourceSwaggerSchema(nameof(Documentation.ValoreCampoProfilo))]
        public string? Valore { get; set; }
        [ResourceSwaggerSchema(nameof(Documentation.ValoriCampoProfilo))]
        public IReadOnlyList<string>? Valori { get; set; }
        [ResourceSwaggerSchema(nameof(Documentation.TipoCampoProfilo))]
        public TipoCampoEnum Tipo { get; set; }
    }


    public class StoreProfiloCommand: IRequest<StoreProfiloCommandResponse>
    {
        public string Id { get; set; }
        public StoreProfilo Profilo { get; set; }
    }
}
