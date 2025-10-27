// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using Pi3.App.RubricaComune.Infrastructure.EF.Entities;
using Pi3.Core.SeedWork;
using Pi3.App.RubricaComune.WebApi.Application.Queries.Corrispondenti.GetEmails;

namespace Pi3.App.RubricaComune.WebApi.Application.Queries.Corrispondenti
{
    public enum Tipi
    {
        UnitaOrganizzativa,
        RaggruppamentoFunzionale
    }

    public class Corrispondente : ValueObject
    {       
        public string Id { get; init; } = null!;

        public string Codice { get; init; } = null!;

        public string Denominazione { get; init; } = null!;

        public DateTime DataCreazione { get; init; }

        public DateTime DataUltimaModifica { get; init; }

        public Tipi Tipo { get; init; }

        public string? Indirizzo { get; init; } = null!;

        public string? Telefono { get; init; } = null!;

        public string? Fax { get; init; } = null!;

        public string? Citta { get; init; } = null!;

        public string? CAP { get; init; } = null!;

        public string? Provincia { get; init; } = null!;

        public string? Nazione { get; init; } = null!;

        public string? CodiceFiscale { get; init; } = null!;

        public string? PartitaIva { get; init; } = null!;

        public string? UrlApiInteroperabilita { get; init; } = null!;

        public string? AOO { get; init; } = null!;

        public string? Amministrazione { get; init; } = null;

        public bool? Pubblicato { get; init; }
        public string? RubricaEsterna { get; set; }
        public string? Canale { get; set; }
        public string? Email { get; set; }
        public List<Email> Emails { get; set; }

        public static void CreateMapFromElementoRubricaEntity(IMapperConfigurationExpression mapperConfigurationExpression)
        {
            mapperConfigurationExpression.CreateMap<ElementoRubricaEntity, Pi3.App.RubricaComune.WebApi.Application.Queries.Corrispondenti.Corrispondente>()
            .ForMember(dest => dest.Denominazione, opt => opt.MapFrom(src => src.DESCRIZIONE.ToString()))
            .ForMember(dest => dest.UrlApiInteroperabilita, opt => opt.MapFrom(src => src.URL))
            .ForMember(dest => dest.Pubblicato, opt => opt.MapFrom(src => src.CHA_PUBBLICATO == "1"))
            .ForMember(dest => dest.CodiceFiscale, opt => opt.MapFrom(src => src.VAR_COD_FISC))
            .ForMember(dest => dest.PartitaIva, opt => opt.MapFrom(src => src.VAR_COD_PI))
            .ForMember(dest => dest.AOO, opt => opt.MapFrom(src => src.AOO))
            .ForMember(dest => dest.Amministrazione, opt => opt.MapFrom(src => src.AMMINISTRAZIONE))
            .ForMember(dest => dest.Indirizzo, opt => opt.MapFrom(src => src.INDIRIZZO))
            .ForMember(dest => dest.Nazione, opt => opt.MapFrom(src => src.NAZIONE))
            .ForMember(dest => dest.CAP, opt => opt.MapFrom(src => src.CAP))
            .ForMember(dest => dest.Citta, opt => opt.MapFrom(src => src.CITTA))
            .ForMember(dest => dest.Provincia, opt => opt.MapFrom(src => src.PROVINCIA))
            .ForMember(dest => dest.Telefono, opt => opt.MapFrom(src => src.TELEFONO))
            .ForMember(dest => dest.Tipo, opt => opt.MapFrom(src => src.TIPOCORRISPONDENTE == "UO" ? Tipi.UnitaOrganizzativa : Tipi.RaggruppamentoFunzionale));
        }
    }
}
