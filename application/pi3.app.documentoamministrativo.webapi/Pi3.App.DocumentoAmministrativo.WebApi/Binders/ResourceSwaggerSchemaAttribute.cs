// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.App.DocumentoAmministrativo.WebApi.Resources;
using Swashbuckle.AspNetCore.Annotations;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Binders;


public class ResourceSwaggerSchemaAttribute: SwaggerSchemaAttribute
{
    public ResourceSwaggerSchemaAttribute(string resourceId) : base()
    {
        var strContent = Documentation.ResourceManager.GetString(resourceId);

        Description = strContent;

    }

}
