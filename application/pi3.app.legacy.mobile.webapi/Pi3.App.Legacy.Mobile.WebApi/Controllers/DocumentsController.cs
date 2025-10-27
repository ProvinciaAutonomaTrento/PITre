// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using Swashbuckle.AspNetCore.Annotations;
using Pi3.Infrastructure.Services.AAC;
using Microsoft.AspNetCore.Authorization;

using EXCEPTIONS = Pi3.App.Legacy.Mobile.Shared.Exceptions;
using QUERY = Pi3.App.Legacy.Mobile.ApiHandler.Handlers.Queries;
using COMMAND = Pi3.App.Legacy.Mobile.ApiHandler.Handlers.Commands;
using RESULTS = Pi3.App.Legacy.Mobile.ApiHandler.Models.CommandResults;
using RESPONSE = Pi3.App.Legacy.Mobile.ApiHandler.Models.QueryResponses;
using DTO = Pi3.App.Legacy.Mobile.ApiHandler.Models.DTOs;
using DocsPaVO.Mobile.Requests;
using DocsPaVO.Mobile.Responses;
using System.Net;
using Newtonsoft.Json.Linq;
using Pi3.App.Legacy.Mobile.WebApi.Model;
using Pi3.App.Legacy.Mobile.WebApi.Manager;

namespace Pi3.App.Legacy.Mobile.WebApi.Controllers;

//[Authorize(policy: Policies.PITRE)]
[Authorize()]
[ApiVersion(1.0)]
//[Route("api/v{version:apiVersion}/[controller]")]
[Route("api/[controller]")]
[ApiController]
public class DocumentsController( 
    ILogger<DocumentsController> logger,
    IConfiguration configuration,
    IMediator mediator) : ControllerBase
{
    readonly ILogger<DocumentsController> _logger = logger;
    readonly IConfiguration _configuration = configuration;
    readonly IMediator _mediator = mediator;

    [HttpGet("Details")]
    [Produces(System.Net.Mime.MediaTypeNames.Application.Json)]
    [SwaggerOperation( OperationId = "Documents_GetDocInfo", Tags = ["Documents"])]
    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(DTO.Documents.GetDocumentInfoDto), ContentTypes = [System.Net.Mime.MediaTypeNames.Application.Json])]
    [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails), ContentTypes = [System.Net.Mime.MediaTypeNames.Application.Json])]
    [SwaggerResponse(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails), ContentTypes = [System.Net.Mime.MediaTypeNames.Application.Json])]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails), ContentTypes = [System.Net.Mime.MediaTypeNames.Application.Json])]
    public async Task<ActionResult<DTO.Documents.GetDocumentInfoDto>> GetDocInfo( 
        [FromQuery(Name = "idDoc")]string parap_IdDocumento,
        [FromQuery(Name = "idEvento")] string? param_IdEvento,
        [FromQuery(Name = "idTrasm")] string? param_IdTrasmissione,
        [FromHeader(Name = "AuthToken")] string token)
    {
        this._logger.LogInformation("Documents GetDocInfo");

        //if ( !long.TryParse(parap_IdDocumento, out long idDocumento) || idDocumento == 0)
        //{
        //    this._logger.LogError("Conversione del parametro IdDocumento fallito: {Param}", parap_IdDocumento);
        //    throw new EXCEPTIONS.RequestParamNotFoundException("idDoc");
        //}

        //this._logger.LogDebug("Recupero i dettagli del documento");
        //QUERY.GetDocumentInfoQuery request = new (idDocumento);
        //DTO.Documents.GetDocumentInfoDto result =  await this._mediator.Send(request);
        //this._logger.LogDebug("Dettagli del documento recuperati");

        //if ( !String.IsNullOrEmpty(param_IdTrasmissione) )
        //{
        //    if ( !long.TryParse(param_IdTrasmissione, out long idTrasmissione) )
        //    {
        //        this._logger.LogError("Conversione del parametro IdTrasmissione fallito: {Param}", param_IdTrasmissione);
        //        throw new EXCEPTIONS.RequestParamNotFoundException("idTrasm");
        //    }
        //    if ( idTrasmissione > 0 )
        //    {
        //        QUERY.GetUserTrasmissionById requestTrasmissione = new(idTrasmissione);
        //        DTO.Trasmissions.GetTrasmissionByIdDTO trasmissione = await this._mediator.Send(requestTrasmissione);
        //        result.TrasmInfo = trasmissione;
        //    }
        //}
        //else if ( !String.IsNullOrEmpty(param_IdEvento))
        //{
        //    if ( !long.TryParse(param_IdEvento, out long idEvento) )
        //    {
        //        this._logger.LogError("Conversione del parametro IdTrasmissione fallito: {Param}", param_IdEvento);
        //        throw new EXCEPTIONS.RequestParamNotFoundException("idTrasm");
        //    }
        //    if ( idEvento > 0 )
        //    {
        //        string setDataVistaGrid = this._configuration["SET_DATA_VISTA_GRD"] ?? "0";
        //        if(setDataVistaGrid != "2" )
        //        {
        //            COMMAND.RimuoviNotificaCommand command = new(idEvento);
        //            await this._mediator.Send(command);
        //        }

        //    }
        //}
        //string token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
        GetDocInfoRequest request = new GetDocInfoRequest();
        request.IdDoc = parap_IdDocumento;
        request.IdEvento = param_IdEvento;
        request.IdTrasm = param_IdTrasmissione;
        GetDocInfoResponse retval = await MobileManager.getDocInfo(token, request);
        //response = Request.CreateResponse(HttpStatusCode.OK, retval);
        return Ok(retval);

        //return Ok(result);
    }

    //[HttpPost("File")]
    [HttpGet("File")]
    [SwaggerOperation(OperationId = "Documents_GetFile", Tags = ["Documents"])]
    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(RESPONSE.Documents.GetFileByIdDocumentResponse), ContentTypes = [System.Net.Mime.MediaTypeNames.Application.Json])]
    [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails), ContentTypes = [System.Net.Mime.MediaTypeNames.Application.Json])]
    [SwaggerResponse(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails), ContentTypes = [System.Net.Mime.MediaTypeNames.Application.Json])]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails), ContentTypes = [System.Net.Mime.MediaTypeNames.Application.Json])]
    public async Task<IActionResult> GetFile( [FromQuery(Name = "idDoc")]string idDocumento,
        [FromHeader(Name = "AuthToken")] string token, string getSignedFile = "0")
    {
        //QUERY.GetFileByIdDocumentRequest request = new (idDocumento);
        //RESPONSE.Documents.GetFileByIdDocumentResponse response = await this._mediator.Send(request);
        string temporaryRootPath = configuration.GetSection("FirmaRemotaServiceOptions:SpoolFolder").Value;
        GetFileResponse response = await MobileManager.getFile(token, idDocumento, getSignedFile, temporaryRootPath);
        return Ok(response);
    }



    //[HttpGet("Fascicolo")]
    [Route("FascInfo")]
    public async Task<ActionResult<GetFascInfoResponse>> GetFascicoloInfo(
        [FromQuery(Name = "idFasc")] long idFascicolo, 
        [FromQuery(Name = "idTrasm")]long idTrasmissione,
        [FromHeader(Name = "AuthToken")] string token)
    {
        //QUERY.GetFascicoloInfoByIdQuery request = new (idFascicolo, idTrasmissione);
        //RESPONSE.Fascicoli.GetFascicoloInfoByIdResponse fasicolo = await this._mediator.Send(request);
        //RESPONSE.GetTrasmissioneByIdResponse trasmissione = await this._mediator.Send(new QUERY.GetTrasmissionById(idTrasmissione));
        //return Ok(new
        //{
        //    fasicolo.FascInfo,
        //    TrasmInfo = trasmissione.Trasmissione
        //});
        //HttpResponseMessage response = null;
        //string token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
        GetFascInfoRequest request = new GetFascInfoRequest();
        request.IdFasc = idFascicolo.ToString();
        request.IdTrasm = idTrasmissione.ToString();
        GetFascInfoResponse response = await MobileManager.getFascInfo(token, request);
        //response = Request.CreateResponse(HttpStatusCode.OK, retval);
        return Ok(response);

    }

    [Route("RimuoviNotifica")]
    //[ResponseType(typeof(bool))]
    [HttpPost]
    public async Task<bool> rimuoviNotifica(string idEvento)
    {
        bool retval = await MobileManager.rimuoviNotifica(idEvento);
        //response = Request.CreateResponse(HttpStatusCode.OK, retval);
        return retval;
    }

    [Route("AdlAction")]
    //[ResponseType(typeof(ADLActionResponse))]
    [HttpPost]
    public async Task<ADLActionResponse> AdlAction(AdlActionRequest request,
        [FromHeader(Name = "AuthToken")] string token)
    {
        ADLActionResponse retval = await MobileManager.ADLAction(token, request);
        return retval;
    }

    [Route("Condividi")]
    //[ResponseType(typeof(string))]
    [HttpGet]
    public async Task<string> condividiDocumento(string idpeople, string idDocumento,
        [FromHeader(Name = "AuthToken")] string token)
    {
        string retval = await MobileManager.getTokenCondivisioneDocumento(token, idpeople, idDocumento);
        return retval;
    }

    [Route("DocumentoCondiviso")]
    //[ResponseType(typeof(DocCondivisoResponse))]
    [HttpPost]
    public async Task<DocCondivisoResponse> docCondiviso(string chiaveDoc,
        [FromHeader(Name = "AuthToken")] string token)
    {
        DocCondivisoResponse retval = await MobileManager.ctrlTokenCondivisioneDoc(token, chiaveDoc);
        return retval;
    }

}
