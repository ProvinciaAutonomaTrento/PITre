// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using pi3.Core.Contracts.DocumentoAmministrativo.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pi3.Core.Contracts.DocumentoAmministrativo;

public interface ISearchService
{
    Task<GetElementsQueryResults> GetElements(GetElementsQuery query);
}
