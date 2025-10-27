// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.App.AggregazioneDocumentale.WebApi.Resources;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json;

namespace Pi3.App.AggregazioneDocumentale.WebApi.Binders;

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
