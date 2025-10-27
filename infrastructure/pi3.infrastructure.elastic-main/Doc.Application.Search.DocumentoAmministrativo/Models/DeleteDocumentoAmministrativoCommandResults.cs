// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;

namespace Pi3.Search.DocumentoAmministrativo.Models;

public class DeleteDocumentoAmministrativoCommandResults : ValueObject
{
    public RequestStatus RequestStatus { get; init; }
}
