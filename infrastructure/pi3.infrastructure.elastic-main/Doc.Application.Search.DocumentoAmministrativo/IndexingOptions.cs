// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doc.Search.DocumentoAmministrativo;

public enum DocumentoAmministrativoIndexTypesEnum
{
    Tenant,
    Tenant_CreationDate,
    Tenant_CreationDate_Profile,
    Tenant_AnnoRegistrazione_Registro,
    Tenant_Classification
}

public class DocumentoAmministrativoIndexingOptions
{
    [Required]
    public bool BlobIndexingEnabled { get; set; }

    [Required]
    [MinLength(1)]
    public List<DocumentoAmministrativoIndexTypesEnum> IndexTypes { get; set; }

    //[Required]
    //[MinLength(1)]
    //public List<DocumentoAmministrativoIndexPartition> Partitions { get; set; }
}

public class IndexingOptions
{
    [Required]
    public string ElasticSearchUri { get; set; }

    [Required]
    public DocumentoAmministrativoIndexingOptions DocumentoAmministrativoIndexingOptions { get; set; }
}
