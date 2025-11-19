// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pi3.App.RubricaComune.WebApi.Application.Commands.Corrispondenti;
using Pi3.App.RubricaComune.WebApi.Application.Commands.Corrispondenti.Create;
using Pi3.App.RubricaComune.WebApi.Application.Commands.Corrispondenti.Delete;
using Pi3.App.RubricaComune.WebApi.Application.Commands.Corrispondenti.Pubblica;
using Pi3.App.RubricaComune.WebApi.Application.Commands.Corrispondenti.PubblicaAggiornamento;
using Pi3.App.RubricaComune.WebApi.Application.Commands.Corrispondenti.Update;
using Pi3.App.RubricaComune.WebApi.Application.Queries.Corrispondenti.GetById;
using Pi3.App.RubricaComune.WebApi.Application.Queries.Corrispondenti.Search;
using Pi3.App.RubricaComune.WebApi.Models;
using Pi3.Infrastructure.Services.AAC;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Pi3.App.RubricaComune.WebApi.Controllers
{
    [ApiController]
    [ApiVersion("1")]
    [Authorize(policy: Policies.PITRE)]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class CorrispondentiController : Controller
    {
        #region Public Members

        public CorrispondentiController(
            ILogger<CorrispondentiController> logger,
            IMediator mediator
            )
        {
            this._logger = logger;
            this._mediator = mediator;
        }

        [HttpPost()]
        [Route("Search")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Pi3.App.RubricaComune.WebApi.Application.Queries.Corrispondenti.Search.SearchResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<Pi3.App.RubricaComune.WebApi.Application.Queries.Corrispondenti.Search.SearchResponse>> SearchCorrispondenti([FromBody][Required] SearchRequest request)
        {
            var response = await this._mediator.Send(request);

            return Ok(response);
        }

        [HttpGet()]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Application.Queries.Corrispondenti.Corrispondente))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<Application.Queries.Corrispondenti.Corrispondente>> GetCorrispondenteById([FromRoute] string id)
        {
            var response = await this._mediator.Send(new GetByIdRequest() { Id = id });

            return Ok(response.Corrispondente);
        }

        [HttpPost()]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreateCorrispondenteResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<Application.Queries.Corrispondenti.Corrispondente>> CreateCorrispondente([FromBody][Required] CreateCorrispondenteRequest request)
        {
            var response = await this._mediator.Send(request);

            return StatusCode(StatusCodes.Status201Created,
                (await this._mediator.Send(new GetByIdRequest()
                {
                    Id = response.Id
                })).Corrispondente);
        }

        [HttpPost()]
        [Route("Pubblicazione")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(PubblicaCorrispondenteResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<PubblicaCorrispondenteResponse>> PubblicaCorrispondente([FromBody][Required] PubblicaCorrispondenteRequest request)
        {
            var response = await this._mediator.Send(request);

            return StatusCode(StatusCodes.Status201Created,
                (await this._mediator.Send(new GetByIdRequest()
                {
                    Id = response.Id
                })).Corrispondente);
        }

        [HttpPut()]
        [Route("{id}/Pubblicazione")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<PubblicaCorrispondenteResponse>> PubblicaAggiornamentoCorrispondente([FromRoute] string id, [FromBody][Required] DatiAggiornamento datiAggiornamento)
        {
            await this._mediator.Send(new PubblicaAggiornamentoRequest()
            {
                Id = id,
                DatiAggiornamento = datiAggiornamento
            });

            return NoContent();
        }

        [HttpPost()]
        [Route("{id}/Emails")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult> AddEmail([FromRoute] string id, [FromBody][Required] Pi3.App.RubricaComune.WebApi.Application.Commands.Corrispondenti.AddEmail.DatiEmail datiEmail)
        {
            await this._mediator.Send(new Pi3.App.RubricaComune.WebApi.Application.Commands.Corrispondenti.AddEmail.AddEmailRequest()
            {
                Id = id,
                DatiEmail = datiEmail
            });

            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpPut()]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult> UpdateCorrispondente([FromRoute] string id, [FromBody][Required] DatiCorrispondente request)
        {
            await this._mediator.Send(new UpdateCorrispondenteRequest()
            {
                Id = id,
                DatiCorrispondente = request
            });

            return NoContent();
        }

        [HttpGet()]
        [Route("{id}/Emails")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Pi3.App.RubricaComune.WebApi.Application.Queries.Corrispondenti.GetEmails.Email))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<IReadOnlyList<Pi3.App.RubricaComune.WebApi.Application.Queries.Corrispondenti.GetEmails.Email>>> GetEmails([FromRoute] string id)
        {

            var response = await this._mediator.Send(new Pi3.App.RubricaComune.WebApi.Application.Queries.Corrispondenti.GetEmails.GetEmailsRequest()
            {
                Id = id
            });

            return Ok(response.Emails);
        }

        [HttpPut()]
        [Route("{id}/Emails/{email}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult> UpdateEmail([FromRoute] string id, [FromRoute] string email, [FromBody][Required] Pi3.App.RubricaComune.WebApi.Application.Commands.Corrispondenti.UpdateEmail.DatiEmail datiEmail)
        {
            await this._mediator.Send(new Pi3.App.RubricaComune.WebApi.Application.Commands.Corrispondenti.UpdateEmail.UpdateEmailRequest()
            {
                Id = id,
                Email = email,
                DatiEmail = datiEmail
            });

            return NoContent();
        }

        [HttpDelete()]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult> DeleteCorrispondente([FromRoute] string id)
        {
            await this._mediator.Send(new DeleteCorrispondenteRequest()
            {
                Id = id
            });

            return NoContent();
        }


        [HttpDelete()]
        [Route("{id}/Emails/{email}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult> DeleteEmail([FromRoute] string id, [FromRoute] string email)
        {
            await this._mediator.Send(new Pi3.App.RubricaComune.WebApi.Application.Commands.Corrispondenti.DeleteEmail.DeleteEmailRequest()
            {
                Id = id,
                Email = email
            });

            return NoContent();
        }

        #endregion

        #region Private Members

        protected readonly ILogger<CorrispondentiController> _logger;
        protected readonly IMediator _mediator;

        #endregion
    }
}
