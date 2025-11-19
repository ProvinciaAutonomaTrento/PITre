// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Swashbuckle.AspNetCore.Annotations;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using Newtonsoft.Json;
using System.Text;
using Pi3.Infrastructure.Services.AAC;
using Microsoft.AspNetCore.Authorization;

using QUERY = Pi3.App.Legacy.Mobile.ApiHandler.Handlers.Queries;
using DTO = Pi3.App.Legacy.Mobile.ApiHandler.Models.DTOs;

namespace Pi3.App.Legacy.Mobile.WebApi.Controllers;

//[Authorize(policy: Policies.PITRE)]
[Authorize()]
[ApiExplorerSettings(IgnoreApi = true)]
[Route("api/[controller]")]
[ApiController]
public class AppSettingsController(
    ILogger<AppSettingsController> logger,
    IMediator mediator,
    IConfiguration configuration ) : ControllerBase
{
    readonly ILogger<AppSettingsController> _logger = logger;
    readonly IMediator _mediator = mediator;
    readonly IConfiguration _configuration = configuration;


    [HttpGet("Instance")]
    [SwaggerOperation(OperationId = "AppSettings_GetInstanceList",  Tags = ["AppSettings"] )]
    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(IEnumerable<DTO.Instances.Instance>), ContentTypes = [System.Net.Mime.MediaTypeNames.Application.Json])]
    [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails), ContentTypes =[System.Net.Mime.MediaTypeNames.Application.Json])]
    [SwaggerResponse(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails), ContentTypes = [System.Net.Mime.MediaTypeNames.Application.Json])]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails), ContentTypes = [System.Net.Mime.MediaTypeNames.Application.Json])]
    public async Task<IActionResult> GetInstanceList()
    {
        QUERY.GetInstanceList request = new();
        IEnumerable<DTO.Instances.Instance> result = await this._mediator.Send( request );

        if (User?.Identity?.IsAuthenticated != null && true)
        {
            this._logger.LogInformation("User: {User}", User.Identity.Name);
        }
        else {
            this._logger.LogInformation("User not found:");
        }

        return Ok(result);
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpGet("DemoProducerRabbit")]
    public IActionResult TestWriteMessageRabbitMQ()
    {
        string? host = this._configuration["RABBITMQ_HOST"];
        string? username = this._configuration["RABBITMQ_USER"];
        string? password = this._configuration["RABBITMQ_PASSWORD"];
        string parapm_port = this._configuration["RABBITMQ_PORT"] ?? "5672";


        try
        {
            this._logger.LogDebug("Rabbit URL: {host}:{port} ", host, parapm_port);
            int port = int.Parse(parapm_port);

            var factory = new ConnectionFactory()
            {
                UserName = username,
                Password = password,
                HostName = host,
                Port = port
            };
            this._logger.LogDebug("Creo la connessione...");
            using IConnection connection = factory.CreateConnection();
            using IModel? channel = connection.CreateModel();

            this._logger.LogDebug("Recupero coda...");
            channel.QueueDeclare(queue: "demoQueue",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

            var json = JsonConvert.SerializeObject(new { Nome = "Utente", Dominio = "Test", Valore = "12345" });
            var body = Encoding.UTF8.GetBytes(json);

            this._logger.LogDebug("Pubblico messaggio...");
            channel.BasicPublish(exchange: "",
                            routingKey: "demoQueue",
                            basicProperties: null,
                            body: body);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "{Message}", ex.Message);
            throw;
        }

        

        return Ok();
    }


}
