// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using System.ComponentModel.DataAnnotations;

namespace Pi3.App.RubricaComune.WebApi.Application.Queries.Corrispondenti.GetEmails
{
    public class Email : ValueObject
    {
        public string Indirizzo { get; init; } = null!;

        public bool? Preferita { get; init; }

        public string? Note { get; init; }
    }
}
