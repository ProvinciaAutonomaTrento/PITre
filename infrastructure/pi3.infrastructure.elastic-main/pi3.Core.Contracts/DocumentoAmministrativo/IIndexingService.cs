// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pi3.Core.Contracts.DocumentoAmministrativo;

public interface IIndexingService
{
    Task<bool> AddOrEditDocument<T>(T document, string index);

    Task<bool> DeleteDocument(string id, string index);
}
