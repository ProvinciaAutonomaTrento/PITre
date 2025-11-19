// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later

using DocsPaVO.Settings;
using DocsPaWS;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;
using SoapCore;
using System.Configuration;

namespace Pi3.App.Legacy.Admin.WebApi;

public partial class TestStartup
{
    public IConfiguration Configuration { get; }
    public TestStartup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        var parametri = new AppSettings();
        Configuration.GetSection("Parametri").Bind(parametri);
        AppSettings.Instance = parametri;

        services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));
        services.AddHealthChecks()
           .AddCheck("BaseHealthCheck", () => HealthCheckResult.Healthy());



        //builder.Services.AddReverseProxy()
        //    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
        services.AddSoapCore();
        services.TryAddSingleton<IDocsPaWS, DocsPaWS.DocsPaWS>();
        services.AddMvc();

    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
    {
        app.UseRouting();

        app.UseMiddleware<ReadHeaderMiddleware>();

        //const string healthCheckPattern = "/healthz";
        //const string readinessCheckPattern = "/readiness";
        //const string startupCheckPattern = "/startup";
        //const string metricsPattern = "/metrics";

        //app.MapHealthChecks(healthCheckPattern);
        //app.MapHealthChecks(readinessCheckPattern);
        //app.MapHealthChecks(startupCheckPattern);

        //app.UseWhen(
        //    context => !context.Request.Path.StartsWithSegments(healthCheckPattern)
        //    && !context.Request.Path.StartsWithSegments(readinessCheckPattern)
        //    && !context.Request.Path.StartsWithSegments(startupCheckPattern)
        //    && !context.Request.Path.StartsWithSegments(metricsPattern),
        //    builder =>
        //        builder.UseMiddleware<ClaimsPrincipalActivatorMiddleware>());


        app.UseEndpoints(endpoints => {
            endpoints.UseSoapEndpoint<IDocsPaWS>("/DocsPaWS.svc", new SoapEncoderOptions(), SoapSerializer.DataContractSerializer);
            endpoints.UseSoapEndpoint<IDocsPaWS>("/DocsPaWS.asmx", new SoapEncoderOptions(), SoapSerializer.XmlSerializer);
        });

    }
}
