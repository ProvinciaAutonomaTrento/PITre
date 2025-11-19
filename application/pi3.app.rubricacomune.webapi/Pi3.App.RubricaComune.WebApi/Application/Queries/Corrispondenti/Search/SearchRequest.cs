// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Pi3.Core.SeedWork;
using System.ComponentModel.DataAnnotations;

namespace Pi3.App.RubricaComune.WebApi.Application.Queries.Corrispondenti.Search
{
    public class SearchRequest : ValueObject, IRequest<SearchResponse>
    {
        [Required]
        [Range(0, Int32.MaxValue)]
        public int Pagina { get; init; }

        [Required]
        [Range(1, 50)]
        public int ElementiPerPagina { get; init; }

        [Required]
        public IReadOnlyList<CriterioRicerca>? CriteriRicerca { get; init; } = null;

        public IReadOnlyList<CriterioOrdinamento>? CriteriOrdinamento { get; init; } = null;
    }

    public enum CampiRicercaEnum
    {
        Codice,
        Denominazione,
        Indirizzo,
        Telefono,
        CodiceFiscale,
        PartitaIva,
        Fax,
        Citta,
        Cap,
        Provincia,
        Nazione,
        Aoo,
        Pubblicato,
        Email,
        Url,
        RubricaEsterna,
        SoloRubricaEsterna
    }

    public enum TipiRicercaParolaEnum
    {
        ParteDellaParola,
        ParolaIntera,
        ParolaIniziaCon,
    }

    public class CriterioRicerca : ValueObject
    {
        public TipiRicercaParolaEnum TipoRicercaParola { get; init; }

        public CampiRicercaEnum Campo { get; init; }

        public string? Valore { get; init; } = null;

        public string ToSql()
        {
            string pattern = string.Empty;

            switch(this.TipoRicercaParola)
            {
                case TipiRicercaParolaEnum.ParolaIntera:
                    pattern = $"{this.Valore?.ToUpper()}";
                    break;
                case TipiRicercaParolaEnum.ParolaIniziaCon:
                    pattern = $"{this.Valore?.ToUpper()}%";
                    break;
                case TipiRicercaParolaEnum.ParteDellaParola:
                    pattern = $"%{this.Valore?.ToUpper()}%";
                    break;
                default:
                    pattern = $"%{this.Valore?.ToUpper()}%";
                    break;
            }

            return pattern;
        }

    }

    public enum TipiOrdinamentoEnum
    {
        Asc,
        Desc,
    }

    public class CriterioOrdinamento : ValueObject
    {
        public CampiRicercaEnum Campo { get; init; }

        public TipiOrdinamentoEnum Tipo { get; init; }
    }
}
