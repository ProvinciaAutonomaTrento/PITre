// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;

namespace Pi3.App.InteropPitre.WebApi.Infrastructure.Services.CommonAddressBook
{
    public enum TipiEnum
    {
        UnitaOrganizzativa,
        RaggruppamentoFunzionale
    }

    public class Corrispondente
    {
        public string Id { get; init; } = null!;

        public string Codice { get; init; } = null!;

        public string Denominazione { get; init; } = null!;

        public DateTime? DataCreazione { get; init; }

        public DateTime? DataUltimaModifica { get; init; }

        public TipiEnum Tipo { get; init; }

        public string? Indirizzo { get; init; } = null;

        public string? Telefono { get; init; } = null;

        public string? Fax { get; init; } = null;

        public string? Citta { get; init; } = null;

        public string? CAP { get; init; } = null;

        public string? Provincia { get; init; } = null;

        public string? Nazione { get; init; } = null;

        public string? CodiceFiscale { get; init; } = null;

        public string? PartitaIva { get; init; } = null;

        public string? UrlApiInteroperabilita { get; init; } = null;

        public string? AOO { get; init; } = null;

        public string? Amministrazione { get; init; } = null;

        public bool? Pubblicato { get; init; }
    }

}
