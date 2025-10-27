// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.ElementAggregate.Factories
{
    internal static class ElementFactory
    {
        public static A Create<A>() where A : Element
        {
            var type = typeof(A);
            var constructor = type.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, Array.Empty<Type>());
            if (constructor == null)
                throw new NotSupportedPi3Exception();

            return (A)constructor.Invoke(null);
        }
    }
}
