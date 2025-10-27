// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;

namespace Pi3.App.RubricaComune.WebApi.Application.Queries.Corrispondenti.GetById
{
    public class GetByIdResponse : ValueObject
    {
        public Corrispondente Corrispondente { get; init; } = null!;
    }
}
