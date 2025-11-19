// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Queries.GetDocumentStream
{
    public class GetDocumentStreamQueryResults
    {
        public System.IO.Stream Stream { get; init; }

        public string ContentType { get; init; }
    }

    public enum OutputFormatsEnum
    {
        ToPdf,
        ToImage
    }

    public class GetDocumentStreamQuery : IRequest<GetDocumentStreamQueryResults>
    {
        [Required]
        public string IdDocument { get; init; }

        public string? IdVersion { get; init; } = null;

        public OutputFormatsEnum? OutputFormat { get; init; } = null;
    }
}
