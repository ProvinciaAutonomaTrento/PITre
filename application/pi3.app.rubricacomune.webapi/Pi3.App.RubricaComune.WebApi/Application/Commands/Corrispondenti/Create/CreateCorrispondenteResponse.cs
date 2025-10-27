// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;

namespace Pi3.App.RubricaComune.WebApi.Application.Commands.Corrispondenti.Create
{
    public class CreateCorrispondenteResponse : ValueObject
    {
        public string Id { get; init; } = null!;
    }
}
