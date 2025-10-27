// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.App.Uploader.WebApi.Resources;
using Swashbuckle.AspNetCore.Annotations;

namespace Pi3.App.Uploader.WebApi.Binders;

public class ResourceSwaggerRequestBodyAttribute : SwaggerRequestBodyAttribute 
{
    public ResourceSwaggerRequestBodyAttribute(string resourceId) : base()
    {
        Description = Documentation.ResourceManager.GetString(resourceId);
    }
}
