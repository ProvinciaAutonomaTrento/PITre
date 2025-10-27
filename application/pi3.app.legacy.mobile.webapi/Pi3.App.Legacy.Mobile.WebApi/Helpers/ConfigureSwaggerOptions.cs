// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Pi3.App.Legacy.Mobile.WebApi.Helpers;

public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider provider;

    public ConfigureSwaggerOptions( IApiVersionDescriptionProvider provider ) => this.provider = provider;

    public void Configure( SwaggerGenOptions options )
    {
        foreach ( var description in provider.ApiVersionDescriptions )
        {
            options.SwaggerDoc(
                description.GroupName,
                new OpenApiInfo()
                {
                    Title = "Example API",
                    Description = "An example API",
                    Version = description.ApiVersion.ToString(),
                });
        }
    }
}
