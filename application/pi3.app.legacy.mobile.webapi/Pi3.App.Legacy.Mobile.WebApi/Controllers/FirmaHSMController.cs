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
using DocsPaVO.Mobile.Requests;
using DocsPaVO.Mobile.Responses;
using System.Net;
using Pi3.App.Legacy.Mobile.WebApi.Manager;
using Pi3.Infrastructure.Tibco.Services.File.FirmaRemota;
using Pi3.Infrastructure.Tibco.Services.File.FirmaRemota2;

namespace Pi3.App.Legacy.Mobile.WebApi.Controllers;

//[Authorize(policy: Policies.PITRE)]
[Authorize()]
[Route("api/[controller]")]
[ApiController]
public class FirmaHSMController(
    ILogger<FirmaHSMController> logger, IFirmaRemota2Service firmaRemotaService) : ControllerBase
{
    readonly ILogger<FirmaHSMController> _logger = logger;


    // aggiungere firma remota
    [Route("FirmaHSM")]
    //[ResponseType(typeof(HSMSignResponse))]
    [HttpPost]
    public async Task<HSMSignResponse> HSMSign(HSMSignRequest request,
        [FromHeader(Name = "AuthToken")] string token)
    {
        HSMSignResponse retval = await MobileManager.hsmSign(token, request, firmaRemotaService);
        return retval;
    }

    [Route("RichiestaOTP")]
    //[ResponseType(typeof(HSMSignResponse))]
    [HttpPost]
    public async Task<HSMSignResponse> HSMSignOTP(HSMSignRequest request,
        [FromHeader(Name = "AuthToken")] string token)
    {
        HSMSignResponse retval = await MobileManager.hsmRequestOTP(token, request, firmaRemotaService);
        return retval;
    }

    [Route("Verifica")]
    //[ResponseType(typeof(HSMSignResponse))]
    [HttpPost]
    public async Task<HSMSignResponse> hsmVerifySign(HSMSignRequest request,
        [FromHeader(Name = "AuthToken")] string token)
    {
        HSMSignResponse retval = await MobileManager.hsmVerifySign(token, request);
        return retval;
    }

    [Route("DettagliDocumento")]
    //[ResponseType(typeof(HSMSignResponse))]
    [HttpPost]
    public async Task<HSMSignResponse> hsmInfoSign(HSMSignRequest request,
        [FromHeader(Name = "AuthToken")] string token)
    {
        HSMSignResponse retval = await MobileManager.hsmInfoSign(token, request);
        return retval;
    }

    [Route("InfoMemento")]
    //[ResponseType(typeof(HSMSignResponse))]
    [HttpGet]
    public async Task<HSMSignResponse> hsmInfoMemento([FromHeader(Name = "AuthToken")] string token)
    {
        HSMSignResponse retval = await MobileManager.hsmGetMementoForUser(token);
        return retval;
    }

    [Route("OTPAbilitato")]
    //[ResponseType(typeof(bool))]
    [HttpGet]
    public async Task<bool> isAllowedOTP([FromHeader(Name = "AuthToken")] string token)
    {
        bool retval = await MobileManager.isAllowedOTP(token);
        return retval;
    }


}
