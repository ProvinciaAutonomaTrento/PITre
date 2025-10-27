// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Google.Apis.Gmail.v1;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pi3.App.Legacy.WebApi.Application.Handlers.CheckMailBox.EmailCallback;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.App.Legacy.WebApi.Application.Services.Google;
using Pi3.App.Legacy.WebApi.Application.Services.Principal;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Email.BoxScanner;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using DocsPaVO.utente;
using Google.Apis.Util;
using Pi3.Core.Extensions;
using System.Security.AccessControl;

namespace Pi3.App.Legacy.WebApi.Controllers
{
    [ApiController]
    [Route("api/{instance}/{codiceAmministrazione}/{codiceRegistro}/[Controller]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ScanGmailController : Controller
    {
        #region Public Members
        
        public ScanGmailController(
            ILogger<ScanGmailController> logger,
            IMediator mediator)
        {
            this._logger = logger;
            this._mediator = mediator;
        }

        [HttpGet()]
        public async Task<IActionResult> Index(
              [FromRoute()] string instance,
              [FromRoute()] string codiceAmministrazione,
              [FromRoute()] string codiceRegistro)
        {
            var result = await this._mediator.Send(
                new ScanGmail(instance, codiceAmministrazione, codiceRegistro));

            return Redirect(result.AuthorizationUrl);
        }

        [HttpGet("AuthCallback")]
        public async Task<IActionResult> AuthCallback([FromQuery] string code, [FromQuery] string? state, 
            [FromRoute] string instance, [FromRoute] string codiceAmministrazione, [FromRoute] string codiceRegistro)
        {
            var scanGmailResult = await this._mediator.Send(
                new ScanGmailCallback(code, state, instance, codiceAmministrazione, codiceRegistro));

            return Ok(scanGmailResult);

        }

        #endregion

        #region Private Members

        protected readonly ILogger<ScanGmailController> _logger;
        protected readonly IMediator _mediator;

        #endregion
    }
}