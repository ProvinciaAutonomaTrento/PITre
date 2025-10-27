// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using Swashbuckle.AspNetCore.Annotations;
using Pi3.Infrastructure.Services.AAC;
using Microsoft.AspNetCore.Authorization;

using COMMAND = Pi3.App.Legacy.Mobile.ApiHandler.Handlers.Commands;
using RESULT = Pi3.App.Legacy.Mobile.ApiHandler.Models.CommandResults;
using EXCEPTIONS = Pi3.App.Legacy.Mobile.Shared.Exceptions;
using DocsPaVO.Mobile.Responses;
using System.Net;
using DocsPaVO.Mobile.Requests;
using Pi3.App.Legacy.Mobile.WebApi.Model;
using Pi3.App.Legacy.Mobile.WebApi.Manager;

namespace Pi3.App.Legacy.Mobile.WebApi.Controllers;

//[Authorize(policy: Policies.PITRE)]
[Authorize()]
[Route("api/[controller]")]
[ApiController]
public class RicercaController(
    ILogger<RicercaController> logger) : ControllerBase
{
    readonly ILogger<RicercaController> _logger = logger;


    [Route("RicercheSalvate")]
    //[ResponseType(typeof(GetRicSalvateResponse))]
    [HttpGet]
    public async Task<GetRicSalvateResponse> getRicSalvate([FromHeader(Name = "AuthToken")] string token)
    {
        GetRicSalvateResponse retval = await MobileManager.getRicSalvate(token);
        return retval;
    }

    [Route("Ricerca")]
    //[ResponseType(typeof(RicercaResponse))]
    [HttpPost]
    public async Task<RicercaResponse> ricerca(Model.RicercaRequest request, [FromHeader(Name = "AuthToken")] string token)
    {
        RicercaResponse retval = await MobileManager.ricerca(token, request);
        return retval;
    }

    [Route("FascRicerca")]
    //[ResponseType(typeof(RicercaFascResponse))]
    [HttpGet]
    public async Task<RicercaFascResponse> ricercaFasc([FromHeader(Name = "AuthToken")] string token, string IdFascicolo, int PageSize, int RequestedPage, string TestoDaCercare = null)
    {
        RicercaFascResponse retval = await MobileManager.getDocsInFasc(token, IdFascicolo, PageSize, RequestedPage, TestoDaCercare);
        return retval;
    }

}
