// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Pi3.Infrastructure.Services.AAC;
using Swashbuckle.AspNetCore.Annotations;

using QUERY = Pi3.App.Legacy.Mobile.ApiHandler.Handlers.Queries;
using DTO = Pi3.App.Legacy.Mobile.ApiHandler.Models.DTOs;
using MODELS = Pi3.App.Legacy.Mobile.Models;
using EXCEPTIONS = Pi3.App.Legacy.Mobile.Shared.Exceptions;
using COMMAND = Pi3.App.Legacy.Mobile.ApiHandler.Handlers.Commands;
using RESULTS = Pi3.App.Legacy.Mobile.ApiHandler.Models.CommandResults;
using RESPONSE = Pi3.App.Legacy.Mobile.ApiHandler.Models.QueryResponses;
using Newtonsoft.Json.Linq;
using DocsPaVO.Mobile.Responses;
using DocsPaVO.Mobile.Requests;
using System.Net;
using Pi3.App.Legacy.Mobile.WebApi.Model;
using Pi3.App.Legacy.Mobile.WebApi.Manager;

namespace Pi3.App.Legacy.Mobile.WebApi.Controllers;

//[Authorize(policy: Policies.PITRE)]
[Authorize()]
[ApiVersion(1.0)]
//[Route("api/v{version:apiVersion}/[controller]")]
[Route("api/[controller]")]
[ApiController]
public class DelegheController(
    ILogger<DelegheController> logger,
    IMediator mediator ) : ControllerBase
{
    private readonly ILogger<DelegheController> _logger = logger;
    private readonly IMediator _mediator = mediator;


    [HttpGet]
    [Route("Deleghe")]
    public async Task<ActionResult<DelegheResponse>> GetDeleghe(
        [FromHeader(Name = "AuthToken")] string token,
        [FromQuery(Name = "statoDelega")] string? parap_statoDelega,
        [FromQuery(Name = "tipoDelega")] string? parap_tipoDelega = "assegnate"
        )
    {
        this._logger.LogInformation("Deleghe -> GetDeleghe");
        //QUERY.GetDelegheRequest request = new(parap_statoDelega, parap_tipoDelega);
        //RESPONSE.Deleghe.GetDelegheResponse result = await this._mediator.Send(request);
        DelegheResponse result = await MobileManager.listaDelegheAssegnate(token, parap_statoDelega, parap_tipoDelega);

        return Ok(result);
    }


    [HttpPost("Delega")]
    [Route("Delega")]
    //public async Task<IActionResult> CreateDelega([FromBody] DTO.Deleghe.Delega delega,
    public async Task<ActionResult<CreaDelegaResponse>> CreateDelega([FromBody] DocsPaVO.Mobile.Delega delega,
                [FromHeader(Name = "AuthToken")] string token)
    {
        //COMMAND.CreaDelegaCommand command = new(delega);
        //RESULTS.Deleghe.CreaDelegaResult result = await this._mediator.Send(command);
        CreaDelegaResponse result = await MobileManager.creaDelega(token, delega);

        return Ok(result);
    }

    //[HttpPost("Revoca")]
    [Route("RevocaDelega")]
    //public async Task<IActionResult> RevocaDeleghe(DTO.Deleghe.Delega delega,
    public async Task<ActionResult<bool>> RevocaDeleghe(DocsPaVO.Mobile.Delega delega,
                [FromHeader(Name = "AuthToken")] string token)
    {
        //COMMAND.RevocaDelegheRequest command = new([delega]);
        //RESULTS.Deleghe.RevocaDelegheResult result = await this._mediator.Send(command);
        //CreaDelegaDaModelloResponse retval = await MobileManager.creaDelegaDaModello(token, request);
        bool result = await MobileManager.revocaDelega(token, delega);

        return Ok(result);
    }

    //[HttpGet("Modelli")]
    [Route("ModelliDelega")]
    public async Task<ActionResult<ListaModelliDelegaResponse>> GetModelliDeleghe([FromHeader(Name = "AuthToken")] string token)
    {
        //QUERY.GetModelliDelegheRequest request = new();
        //RESPONSE.Deleghe.GetModelliDelegheResponse result = await this._mediator.Send(request);
        ListaModelliDelegaResponse result = await MobileManager.getListaModelliDelega(token);

        return Ok(result);
    }

    [Route("DelegaDaModello")]
    //[ResponseType(typeof(CreaDelegaDaModelloResponse))]
    [HttpPost]
    public async Task<CreaDelegaDaModelloResponse> creaDelegaDaModello(CreaDelegaDaModelloRequest request,
        [FromHeader(Name = "AuthToken")] string token)
    {
        CreaDelegaDaModelloResponse retval = await MobileManager.creaDelegaDaModello(token, request);
        return retval;
    }

    [Route("ListaTipiRuolo")]
    //[ResponseType(typeof(ListaTipiRuoloResponse))]
    [HttpGet]
    public async Task<ListaTipiRuoloResponse> getListaTipiRuolo([FromHeader(Name = "AuthToken")] string token)
    {
        ListaTipiRuoloResponse retval = await MobileManager.getListaRuoli(token);
        return retval;
    }

    [Route("ListaUtenti")]
    //[ResponseType(typeof(ListaUtentiResponse))]
    [HttpGet]
    public async Task<ListaUtentiResponse> getListaUtenti(string codiceTipoRuolo, [FromHeader(Name = "AuthToken")] string token)
    {
        ListaUtentiResponse retval = await MobileManager.getListaUtenti(token, codiceTipoRuolo);
        return retval;
    }

    [Route("RicercaUtenti")]
    //[ResponseType(typeof(RicercaUtentiWithRolesResponse))]
    [HttpGet]
    public async Task<RicercaUtentiWithRolesResponse> ricercaUtenti(string descrizione, int numMaxResult, [FromHeader(Name = "AuthToken")] string token)
    {
        RicercaUtentiWithRolesResponse retval = await MobileManager.ricercaUtenti(token, descrizione, numMaxResult);
        return retval;
    }

}
