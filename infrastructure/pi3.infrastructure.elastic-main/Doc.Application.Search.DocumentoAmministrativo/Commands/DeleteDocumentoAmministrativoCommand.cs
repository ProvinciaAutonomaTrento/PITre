// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pi3.Search.DocumentoAmministrativo.Models;

namespace Doc.Application.Search.DocumentoAmministrativo.Commands;

public record DeleteDocumentoAmministrativoCommand(string IdTenant, string IdDocument) : IRequest<DeleteDocumentoAmministrativoCommandResults>;

//public class DeleteDocumentoAmministrativoCommandResults : ValueObject
//{
//    public RequestStatus RequestStatus { get; init; }
//}
