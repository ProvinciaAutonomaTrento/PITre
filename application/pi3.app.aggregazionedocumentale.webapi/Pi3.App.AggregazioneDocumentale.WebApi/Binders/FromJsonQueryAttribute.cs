// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Microsoft.AspNetCore.Mvc;

namespace Pi3.App.AggregazioneDocumentale.WebApi.Binders
{
    public class FromJsonQueryAttribute : ModelBinderAttribute
    {
        public FromJsonQueryAttribute()
        {
            BinderType = typeof(JsonQueryBinder);
        }
    }
}
