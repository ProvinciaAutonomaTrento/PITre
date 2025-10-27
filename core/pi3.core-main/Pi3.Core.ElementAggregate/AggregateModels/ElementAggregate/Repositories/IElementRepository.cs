// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.ElementAggregate.Repositories
{
    public interface IElementRepository<E> :
        IRepository<E, string> where E : Element
    {
        Task<bool> Exists(string idTenant, string idElement);
        Task<E> Get(string idTenant, string idElement, ILoadBehavior[]? loadBehaviors = null);
    }
}
