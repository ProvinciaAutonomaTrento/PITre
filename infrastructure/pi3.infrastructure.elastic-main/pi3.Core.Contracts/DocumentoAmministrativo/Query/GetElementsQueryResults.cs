// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pi3.Core.Contracts.DocumentoAmministrativo.Query;

public class ElementResult : Element
{
    public float Score { get; init; }
}

public class ResultsByElementType
{
    public TypesEnum Type { get; init; }

    public int Total { get; init; }

    public IReadOnlyList<ElementResult> Elements { get; init; }
}

public class GetElementsQueryResults : ValueObject
{
    public IReadOnlyList<ResultsByElementType> Results { get; set; }
}
