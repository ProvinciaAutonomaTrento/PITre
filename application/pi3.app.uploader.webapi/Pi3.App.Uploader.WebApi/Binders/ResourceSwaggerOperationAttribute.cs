// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.App.Uploader.WebApi.Resources;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json;

namespace Pi3.App.Uploader.WebApi.Binders;

internal class SwaggerOperationDefinition {
    public string Summary { get; init; }
    public string Description { get; init; }
    public string OperationId { get; init; }
    public string[] Tags { get; init; } 
    
}
public class ResourceSwaggerOperationAttribute: SwaggerOperationAttribute
{
    public ResourceSwaggerOperationAttribute(string resourceId) : base()
    {
        var strContent = Files.ResourceManager.GetString(resourceId);
        var content = JsonSerializer.Deserialize<SwaggerOperationDefinition>(strContent);

        if (content != null)
        {
            Summary = content.Summary;
            Description = content.Description;
            OperationId = content.OperationId;
            Tags = content.Tags;
        }

    }
}
