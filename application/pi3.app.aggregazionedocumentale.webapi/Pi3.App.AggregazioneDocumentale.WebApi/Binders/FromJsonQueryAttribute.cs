// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
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
