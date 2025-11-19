// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Services.RubricaComune
{
    public class SearchRequest : ValueObject
    {
        [Required]
        [Range(1, Int32.MaxValue)]
        public int Pagina { get; init; }

        [Required]
        [Range(1, 50)]
        public int ElementiPerPagina { get; init; }

        [Required]
        public IReadOnlyList<CriterioRicerca>? CriteriRicerca { get; init; } = null;

        public IReadOnlyList<CriterioOrdinamento>? CriteriOrdinamento { get; init; } = null;

    }

    public class CriterioRicerca : ValueObject
    {
        public TipiRicercaParolaEnum TipoRicercaParola { get; init; }
        public CampiRicercaEnum Campo { get; init; }

        public string? Valore { get; init; } = null;

    }

    public class CriterioOrdinamento : ValueObject
    {

        public CampiRicercaEnum Campo { get; init; }

        public TipiOrdinamentoEnum Tipo { get; init; }

    }

    public enum TipiRicercaParolaEnum
    {
        ParteDellaParola,
        ParolaIntera,
        ParolaIniziaCon,
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


    public enum TipiOrdinamentoEnum
    {
        Asc,
        Desc,
    }

}
