// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using LinqKit;
using MediatR;
using MessagePack;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using Newtonsoft.Json;
using Pi3.App.Legacy.WebApi.Application.Behaviors;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.App.Legacy.WebApi.Exceptions;
using Pi3.App.Legacy.WebApi.Models;
using Pi3.App.Legacy.WebApi.Requests;
using Pi3.App.Legacy.WebApi.Resources;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Services.AAC;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using static System.Net.Mime.MediaTypeNames;

namespace Pi3.App.Legacy.WebApi.Controllers
{
    [Authorize(policy: Policies.PITRE)]
    [ApiController]
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/{instance}/[Controller]")]
    public partial class DataPortalController : ControllerBase
    {
        private readonly ILogger<DataPortalController> _logger;
        private readonly IMediator _mediator;
        private readonly IClaimsPrincipalService _claimsPrincipalService;

        public DataPortalController(
            ILogger<DataPortalController> logger,
            IMediator mediator,
            IClaimsPrincipalService claimsPrincipalService)
        {
            _logger = logger;
            _mediator = mediator;
            _claimsPrincipalService = claimsPrincipalService;
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ExceptionInfo))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ExceptionInfo))]
        [Route("")]
        [Route("{operation}")]
        [Route("{category}/{operation}")]
        public async Task<IActionResult> Handle([FromBody][Required] DataPortalRequest request, CancellationToken cancellationToken)
        {
            var realRequestType = typeof(LoggerBehavior<,>)
                .Assembly
                .GetTypes()
                .FirstOrDefault(t => t.Name == request.RequestContext.Type);

            if (realRequestType == null)
                throw new DataPortalRequestNotFoundPi3Exception(request.RequestContext.Type);

            var realRequest = JsonConvert.DeserializeObject(
                request.RequestContext.AsJson,
                realRequestType,
                new Newtonsoft.Json.JsonSerializerSettings()
                {
                    TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All
                });

            Validator.ValidateObject(realRequest!, new ValidationContext(realRequest!), true);

            var realResponse = await this._mediator.Send(realRequest!);

            if (request.ResponseAsRaw ?? false)
            {
                string jsonResponse = JsonConvert.SerializeObject(realResponse,
                            new Newtonsoft.Json.JsonSerializerSettings()
                            {
                                TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Auto
                            });

                return Content(jsonResponse, "application/json");
            }
            else
            {
                var response = new DataPortalResponse()
                {
                    ResponseContext = new ResponseContext()
                    {
                        Type = realResponse!.GetType().Name,
                        AsJson = JsonConvert.SerializeObject(realResponse,
                            new Newtonsoft.Json.JsonSerializerSettings()
                            {
                                TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Auto
                            }),
                        JsonPropertyMappings = realResponse?.GetType().GetProperties()
                    .Select(p => new ResponseJsonPropertyMapping()
                    {
                        Name = p.Name,
                        Type = p.PropertyType.FullName
                    })
                    .ToList()
                    .AsReadOnly()
                    }
                };

                return Ok(response);
            }
        }
    }
}