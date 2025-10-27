// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.Mobile.Data;
internal static class Helpers
{
    internal static Expression<Func<T, bool>> BuildOrPredicate<T>( IEnumerable<Expression<Func<T, bool>>> conditions )
    {
        Expression<Func<T, bool>>? predicate = null;

        foreach ( var condition in conditions )
        {
            if ( predicate == null )
            {
                predicate = condition;
            }
            else
            {
                var invokedExpr = Expression.Invoke(condition, predicate.Parameters.Cast<Expression>());
                predicate = Expression.Lambda<Func<T, bool>>(Expression.OrElse(predicate.Body, invokedExpr), predicate.Parameters);
            }
        }

        return predicate ?? (t => false);
    }
}
