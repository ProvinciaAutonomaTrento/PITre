// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pi3.Core.AggregateModels.ContentElementAggregate;
using Pi3.Core.AggregateModels.ElementAggregate;

namespace Pi3.Core.AggregateModels.DocumentAggregate
{
    public interface IDocumentRepository : IElementRepository<Document>
    {
    }
}
