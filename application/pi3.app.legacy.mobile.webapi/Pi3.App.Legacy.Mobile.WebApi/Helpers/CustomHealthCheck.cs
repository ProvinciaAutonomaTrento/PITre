// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json;

namespace Pi3.App.Legacy.Mobile.WebApi.Helpers;

public class CustomHealthCheck( IConfiguration configuration ) : IHealthCheck
{
    readonly IConfiguration _configuration = configuration;

    public Task<HealthCheckResult> CheckHealthAsync( HealthCheckContext context, CancellationToken cancellationToken = default )
    {
        bool isAllOk = true;
        var data = new Dictionary<string, object>();

        string? basePath = this._configuration["ASPNETCORE_BASEPATH"];
        if ( !String.IsNullOrWhiteSpace(basePath) )
        {
            data.Add("BasePath", $"OK - {basePath}");
        } else
        {
            data.Add("BasePath", "KO - Nessuna base path configurata, il servizio swagger potrebbe non fiunzionare come previsto");
        }

        var connectionStringsSection = _configuration.GetSection("ConnectionStrings");
        if ( connectionStringsSection.GetChildren().Any() )
        {
            // Nessuna stringa di connessione trovata
            data.Add("Connection String", $"OK - Almeno una rilevata");
        }
        else
        {
            isAllOk = false;
            data.Add("Connection String", $"KO - Nessuna rilevata");
        }

        if ( !isAllOk )
        {
            return Task.FromResult(HealthCheckResult.Unhealthy("Il servizio non è sano", data: data));
        }
        return Task.FromResult(HealthCheckResult.Healthy("Il servizio è sano", data: data));
    }

    public static Task WriteHealthCheckResponse( HttpContext httpContext, HealthReport result )
    {
        httpContext.Response.ContentType = "application/json";

        var response = new
        {
            status = result.Status.ToString(),
            report = result.Entries.Select(e => new { 
                key = e.Key, 
                value = e.Value.Description,
                data = e.Value.Data
            })
        };

        return httpContext.Response.WriteAsync(JsonSerializer.Serialize(response, new JsonSerializerOptions() { WriteIndented = true }));
    }
}
