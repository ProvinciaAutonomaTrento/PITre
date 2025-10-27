// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Indexer.Application.Queries.Request
{

    public enum RequestElementTypesEnum
    {
        DocumentoAmministrativo = 1       
    }

    public class RequestsQueryResults
    {
        public IReadOnlyList<string> RequestIds { get; init; }
    }


    public record RequestQuery(int NTop, string IdTenant, RequestElementTypesEnum ElementType, DateTime? ElementCreationDateFrom = null, DateTime? ElementCreationDateTo = null) : IRequest<RequestsQueryResults>;
}
