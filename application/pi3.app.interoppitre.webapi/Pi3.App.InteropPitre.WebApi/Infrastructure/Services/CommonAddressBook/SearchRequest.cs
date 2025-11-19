// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;

namespace Pi3.App.InteropPitre.WebApi.Infrastructure.Services.CommonAddressBook
{
    public class SearchRequest
    {
        public int Pagina { get; set; }

        public int ElementiPerPagina { get; set; }

        public IReadOnlyList<CriterioRicerca> CriteriRicerca { get; set; }

        public IReadOnlyList<CriterioOrdinamento> CriteriOrdinamento { get; set; }
    }

    public class CriterioRicerca
    {
        public TipiRicercaParolaEnum TipoRicercaParola { get; set; }

        public CampiRicercaEnum Campo { get; set; }

        public string Valore { get; set; }
    }

    public class CriterioOrdinamento
    {
        public CampiRicercaEnum Campo { get; set; }

        public TipiOrdinamentoEnum Tipo { get; set; }
    }

    public enum TipiRicercaParolaEnum
    {
        ParolaIntera,
        ParteDellaParola,
        ParolaIniziaCon
    }

    public enum CampiRicercaEnum
    {
        Codice,
        Descrizione,
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
        Url

    }

    public enum TipiOrdinamentoEnum
    {
        Asc,
        Desc
    }
}
