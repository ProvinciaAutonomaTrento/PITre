// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.Services.Email.BoxScanner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Services.Factory
{
    public interface IFactoryService : IService 
    {
        Task<(bool Success, S? Service)> TryCreate<S>(Func<S, bool> predicate) where S : class, IService;

        Task<S> Create<S>(Func<S, bool> predicate) where S : class, IService;
    }
}
