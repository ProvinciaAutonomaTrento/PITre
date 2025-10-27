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
using Pi3.App.Legacy.Mobile.WebApi.Model;
using Pi3.App.Legacy.Mobile.WebApi.Manager;
using Pi3.Infrastructure.Tibco.Services.File.FirmaRemota;
using Pi3.Infrastructure.Tibco.Services.File.FirmaRemota2;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;

namespace Pi3.App.Legacy.Mobile.WebApi.Controllers;

//[Authorize(policy: Policies.PITRE)]
[Authorize()]
[Route("api/[controller]")]
[ApiController]
public class LibroFirmaController(
    ILogger<LibroFirmaController> logger, IFirmaRemota2Service firmaRemotaService, IConfigurationService configurationService,
        IConfiguration configuration, IMediator mediator, IClaimsPrincipalService claimsPrincipalService, IWebMethodLoggerService webMethodLoggerService) : ControllerBase
{
    readonly ILogger<LibroFirmaController> _logger = logger;


    [Route("LibroFirmaElements")]
    //[ResponseType(typeof(LibroFirmaResponse))]
    [HttpPost]
    public async Task<LibroFirmaResponse> getLibroFirmaElements(Model.LibroFirmaRequest request,
        [FromHeader(Name = "AuthToken")] string token)
    {
        LibroFirmaResponse retval = await MobileManager.GetLibroFirmaElements(token, request);
        return retval;
    }

    [Route("CambiaStatoElemento")]
    //[ResponseType(typeof(LibroFirmaResponse))]
    [HttpPost]
    public async Task<LibroFirmaResponse> CambiaStatoElementoLibroFirma(LibroFirmaCambiaStatoRequest request,
        [FromHeader(Name = "AuthToken")] string token)
    {
        LibroFirmaResponse retval = await MobileManager.CambiaStatoElementoLibroFirma(token, request);
        return retval;
    }

    [Route("CambiaStatoElementiMultipli")]
    //[ResponseType(typeof(LFCambiaStatiResponse))]
    [HttpPost]
    public async Task<LFCambiaStatiResponse> CambiaStatoElementiMultipli(LFCambiaStatiRequest request,
        [FromHeader(Name = "AuthToken")] string token)
    {
        LFCambiaStatiResponse retval = await MobileManager.LFCambiaStati(token, request);
        return retval;
    }

    [Route("RespingiElementi")]
    //[ResponseType(typeof(LibroFirmaResponse))]
    [HttpPost]
    public async Task<LibroFirmaResponse> RespingiSelezionatiElementiLf(Model.LibroFirmaRequest request,
        [FromHeader(Name = "AuthToken")] string token)
    {
        LibroFirmaResponse retval = await MobileManager.RespingiSelezionatiElementiLf(token, request);
        return retval;
    }

    // aggiungere firma remota
    [Route("FirmaElementi")]
    //[ResponseType(typeof(LibroFirmaResponse))]
    [HttpPost]
    public async Task<LibroFirmaResponse> FirmaSelezionatiElementiLf(Model.LibroFirmaRequest request,
        [FromHeader(Name = "AuthToken")] string token)
    {
        LibroFirmaResponse retval = await MobileManager.FirmaSelezionatiElementiLf(token, request, mediator, claimsPrincipalService, webMethodLoggerService);
        return retval;
    }

    [Route("ElementoCades")]
    //[ResponseType(typeof(bool))]
    [HttpGet]
    public async Task<bool> ExistsElementWithSignCades([FromHeader(Name = "AuthToken")] string token)
    {
        bool retval = await MobileManager.ExistsElementWithSignCades(token);
        return retval;
    }

    [Route("ElementoPades")]
    //[ResponseType(typeof(bool))]
    [HttpGet]
    public async Task<bool> ExistsElementWithSignPades([FromHeader(Name = "AuthToken")] string token)
    {
        bool retval = await MobileManager.ExistsElementWithSignPades(token);
        return retval;
    }

    // aggiungere firma remota
    [Route("FirmaElementiHSM")]
    //[ResponseType(typeof(HSMMultiSignResponse))]
    [HttpPost]
    public async Task<HSMMultiSignResponse> HsmMultiSign(HSMSignRequest request, [FromHeader(Name = "AuthToken")] string token)
    {
        string temporaryRootPath = configuration.GetSection("FirmaRemotaServiceOptions:SpoolFolder").Value;
        HSMMultiSignResponse retval = await MobileManager.HsmMultiSign(token, request, firmaRemotaService, configurationService, temporaryRootPath, mediator, claimsPrincipalService, webMethodLoggerService);
        return retval;
    }

    [Route("AutorizzazioneLibroFirma")]
    //[ResponseType(typeof(bool))]
    [HttpGet]
    public async Task<bool> LibroFirmaIsAuthorized([FromHeader(Name = "AuthToken")] string token)
    {
        bool retval = await MobileManager.LibroFirmaIsAutorized(token);
        return retval;
    }

}
