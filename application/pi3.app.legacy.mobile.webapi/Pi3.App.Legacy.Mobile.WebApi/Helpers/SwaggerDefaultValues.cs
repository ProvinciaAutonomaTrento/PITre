// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Globalization;
using System.Text.Json;
using Pi3.App.Legacy.Mobile.Shared.Extensions;
using Microsoft.AspNetCore.Mvc.Controllers;


namespace Pi3.App.Legacy.Mobile.WebApi.Helpers;

public class SwaggerDefaultValues : IOperationFilter
{
    public void Apply( OpenApiOperation operation, OperationFilterContext context )
    {
        // Check if it's a MapIdentityApi action
        if ( context.ApiDescription.ActionDescriptor is not ControllerActionDescriptor controllerActionDescriptor )
        {
            var relativePath = context.ApiDescription.RelativePath ?? "";
            operation.OperationId = $"{GenerateOperationIdBaseString(relativePath)}_{context.ApiDescription.HttpMethod}";
            return;
        }

        // For other controller actions, set OperationId based on controller and action names
        operation.OperationId = $"{controllerActionDescriptor.ControllerName}_{controllerActionDescriptor.ActionName}_{context.ApiDescription.HttpMethod}";


        var operationSummaryResourceKey = $"{operation.OperationId}_Summary";
        string? summary = GetResourceValueByKey(operationSummaryResourceKey);
        if ( summary != null )
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

        ApiDescription apiDescription = context.ApiDescription;

        operation.Deprecated |= apiDescription.IsDeprecated();

        //foreach ( var responseType in context.ApiDescription.SupportedResponseTypes )
        //{
        //    var responseKey = responseType.IsDefaultResponse
        //                      ? "default"
        //                      : responseType.StatusCode.ToString();
        //    var response = operation.Responses[responseKey];

        //    foreach ( var contentType in response.Content.Keys )
        //    {
        //        if ( !responseType.ApiResponseFormats.Any(x => x.MediaType == contentType) )
        //        {
        //            response.Content.Remove(contentType);
        //        }
        //    }
        //}


        foreach ( var parameter in operation.Parameters ?? [] )
        {
            var desc = apiDescription.ParameterDescriptions.First(p => p.Name == parameter.Name);

            parameter.Description ??= desc.ModelMetadata?.Description;

            if ( parameter.Schema.Default == null && desc.DefaultValue != null )
            {
                var json = JsonSerializer.Serialize(
                    desc.DefaultValue,
                    desc.ModelMetadata?.ModelType);
                parameter.Schema.Default = OpenApiAnyFactory.CreateFromJson(json);
            }

            parameter.Required |= desc.IsRequired;
        }
    }

    private static string? GetResourceValueByKey( string resourceKey )
    {
        var culture = CultureInfo.CurrentCulture.Name;
        return Resources.SwaggerMessages.ResourceManager.GetString(resourceKey, new CultureInfo(culture));
    }

    private static string GenerateOperationIdBaseString( string inputString )
    {
        var input = inputString.Split('/');
        if ( input.Length == 0 )
        {
            return string.Empty;
        }

        var controllerName = input[0].FirstCharToUpper();
        var actionName = string.Join("", input.Skip(1).Select(word => word.FirstCharToUpper()));

        return $"{controllerName}_{actionName}";
    }

}