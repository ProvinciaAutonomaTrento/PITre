// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using pi3.Core.Contracts.DocumentoAmministrativo.Query;
using System.ComponentModel.DataAnnotations;

namespace Pi3.Infrastructure.Elastic;

public class LuceneIndexingOptions
{
    [Required]
    public string ElasticSearchUri { get; set; }
    public DocumentoAmministrativoIndexingOptions DocumentoAmministrativoIndexingOptions { get; set; }
}
