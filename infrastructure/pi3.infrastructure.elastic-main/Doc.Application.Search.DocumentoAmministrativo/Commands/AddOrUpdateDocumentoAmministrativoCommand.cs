// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Pi3.Search.DocumentoAmministrativo.Models;

namespace Doc.Application.Search.DocumentoAmministrativo.Commands;

public record AddOrUpdateDocumentoAmministrativoCommand(string IdTenant, string IdDocument) : IRequest<AddOrUpdateDocumentoAmministrativoCommandResults>;

//public class AddOrUpdateDocumentoAmministrativoCommandResults : ValueObject
//{
//    public RequestStatus RequestStatus { get; init; }
//}
