// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using pi3.Core.Contracts.DocumentoAmministrativo.Query;

namespace Pi3.App.Search.WebApi.Application.Queries.Elements.GetElements
{
    //public class ElementResult : Element
    //{
    //    public float Score { get; init; }
    //}

    //public class ResultsByElementType
    //{
    //    public TypesEnum Type { get; init; }

    //    public int Total { get; init; }

    //    public IReadOnlyList<ElementResult> Elements { get; init; }
    //}

    //public class GetElementsQueryResults : ValueObject
    //{
    //    public IReadOnlyList<ResultsByElementType> Results { get; set; }        
    //}

    //public enum TypesEnum
    //{
    //    DocumentoAmministrativo = 1
    //}

    public class GetElementsQuery : IRequest<GetElementsQueryResults>
    {
        public int? Skip { get; init; }

        [Required]
        public int? Take { get; init; }

        [Required]
        public IReadOnlyList<TypesEnum> Types { get; init; }

        public string? Query { get; init; }
    }
}
