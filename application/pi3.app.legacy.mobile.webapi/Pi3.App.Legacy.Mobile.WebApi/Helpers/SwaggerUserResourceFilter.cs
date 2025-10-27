// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Azure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Globalization;
using System.Resources;

namespace Pi3.App.Legacy.Mobile.WebApi.Helpers;

public class SwaggerUserResourceFilter : IOperationFilter
{
    public void Apply( OpenApiOperation operation, OperationFilterContext context )
    {
        var operationSummaryResourceKey = $"{operation.OperationId}_Summary";
        string? summary = GetResourceValueByKey(operationSummaryResourceKey);
        if (summary != null )
        {
            operation.Summary = summary;
        }
        var operationDescriptionResourceKey = $"{operation.OperationId}_Description";
        string? description = GetResourceValueByKey(operationDescriptionResourceKey);
        if ( description != null )
        {
            operation.Description = description;
        }

        foreach ( var response in operation.Responses )
        {
            // Assumi che tu abbia una chiave di risorsa che corrisponde al codice di risposta, ad es., "Response200", "Response404"
            var resourceKey = $"{operation.OperationId}_Response_{response.Key}";
            string? localizedDescription = GetResourceValueByKey(resourceKey);
            if ( localizedDescription != null )
            {
                response.Value.Description = localizedDescription;
            }
        }
    }

    private static string? GetResourceValueByKey( string resourceKey )
    {
        var culture = CultureInfo.CurrentCulture.Name;
        return Resources.SwaggerMessages.ResourceManager.GetString(resourceKey, new CultureInfo(culture));
    }


}
