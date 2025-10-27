// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Microsoft.Extensions.Logging;
using pi3.Core.Contracts.DocumentoAmministrativo;
using pi3.Core.Contracts.DocumentoAmministrativo.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Doc.Search.DocumentoAmministrativo;

public class DocumentoAmministrativoSearchService
{
    private readonly ILogger<DocumentoAmministrativoSearchService> _logger;
    private readonly ISearchService documentoAmministativoQueryService;

    public DocumentoAmministrativoSearchService(ILogger<DocumentoAmministrativoSearchService> logger, ISearchService documentoAmministativoQueryService)
    {
        this._logger = logger;
        this.documentoAmministativoQueryService = documentoAmministativoQueryService;
    }

    public async Task<GetElementsQueryResults> GetElements(GetElementsQuery request) {
        return await documentoAmministativoQueryService.GetElements(request);
    }
}
