// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System.ComponentModel.DataAnnotations;

namespace pi3.Core.Contracts.DocumentoAmministrativo.Query;

public enum DocumentoAmministrativoIndexTypesEnum
{
    Tenant,
    Tenant_CreationDate,
    Tenant_CreationDate_Profile,
    Tenant_AnnoRegistrazione_Registro
}

public class DocumentoAmministrativoIndexingOptions
{
    [Required]
    public List<DocumentoAmministrativoIndexTypesEnum> IndexTypes { get; set; }
}
