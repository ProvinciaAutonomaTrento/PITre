// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.App.AggregazioneDocumentale.WebApi.Resources;
using Swashbuckle.AspNetCore.Annotations;

namespace Pi3.App.AggregazioneDocumentale.WebApi.Binders;

public class ResourceSwaggerRequestBodyAttribute : SwaggerRequestBodyAttribute 
{
    public ResourceSwaggerRequestBodyAttribute(string resourceId) : base()
    {
        Description = Documentation.ResourceManager.GetString(resourceId);
    }
}
