// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using System.ComponentModel.DataAnnotations;

namespace pi3.Core.Contracts.DocumentoAmministrativo.Query;

public enum TypesEnum
{
    DocumentoAmministrativo = 1
}

public class GetElementsQuery : ValueObject
{
    public int? Skip { get; init; }

    [Required]
    public int? Take { get; init; }

    [Required]
    public IReadOnlyList<TypesEnum> Types { get; init; }

    public string? Query { get; init; }
}
