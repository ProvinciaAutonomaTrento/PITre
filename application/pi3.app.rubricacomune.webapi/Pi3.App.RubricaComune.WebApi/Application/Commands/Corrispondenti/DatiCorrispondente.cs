// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System.ComponentModel.DataAnnotations;

namespace Pi3.App.RubricaComune.WebApi.Application.Commands.Corrispondenti
{
    public enum Tipi
    {
        UnitaOrganizzativa,
        RaggruppamentoFunzionale
    }

    public class DatiCorrispondente : ValueObject
    {
        [Required(AllowEmptyStrings = false)]
        public string Denominazione { get; init; } = null!;

        [Required]
        public Tipi Tipo { get; init; }

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

        public string? Amministrazione { get; init; } = null;

        public string? AOO { get; init; } = null;
    }
}
