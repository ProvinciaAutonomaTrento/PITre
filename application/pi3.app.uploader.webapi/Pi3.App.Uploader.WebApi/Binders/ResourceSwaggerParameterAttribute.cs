// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.App.Uploader.WebApi.Resources;
using Swashbuckle.AspNetCore.Annotations;
using System.Reflection;

namespace Pi3.App.Uploader.WebApi.Binders
{
    public class ResourceSwaggerParameterAttribute: SwaggerParameterAttribute
    {
        public ResourceSwaggerParameterAttribute(string resourceId): base()
        {
            Description = Documentation.ResourceManager.GetString(resourceId);
        }
    }
}
