// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
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
using Pi3.App.Legacy.Mobile.WebApi.Model;
using DocsPaVO.Mobile.Requests;
using Pi3.App.Legacy.Mobile.WebApi.Manager;

namespace Pi3.App.Legacy.Mobile.WebApi.Controllers;

//[Authorize(policy: Policies.PITRE)]
[Authorize()]
[Route("api/[controller]")]
[ApiController]
public class TrasmissioniController(
    ILogger<TrasmissioniController> logger) : ControllerBase
{
    readonly ILogger<TrasmissioniController> _logger = logger;


    [Route("TrasmissioneVista")]
    //[ResponseType(typeof(AccettaRifiutaTrasmResponse))]
    [HttpPost]
    public async Task<AccettaRifiutaTrasmResponse> setDataVista_SP(TrasmVistaRequest request, [FromHeader(Name = "AuthToken")] string token)
    {
        AccettaRifiutaTrasmResponse retval = await MobileManager.setDataVistaSP_TV(token, request);
        return retval;
    }

    [Route("AccettaRifiutaTrasm")]
    //[ResponseType(typeof(AccettaRifiutaTrasmResponse))]
    [HttpPost]
    public async Task<AccettaRifiutaTrasmResponse> AccettaRifiutaTrasm(Model.AccettaRifiutaTrasmRequest request, [FromHeader(Name = "AuthToken")] string token)
    {
        AccettaRifiutaTrasmResponse retval = await MobileManager.accettaRifiutaTrasm(token, request);
        return retval;
    }

    [Route("Smista")]
    //[ResponseType(typeof(AccettaRifiutaTrasmResponse))]
    [HttpPost]
    public async Task<AccettaRifiutaTrasmResponse> Smista(SmistaRequest request, [FromHeader(Name = "AuthToken")] string token)
    {
        AccettaRifiutaTrasmResponse retval = await MobileManager.smista(token, request);
        return retval;
    }

    [Route("ListaModelliTrasmissione")]
    //[ResponseType(typeof(ListaModelliTrasmResponse))]
    [HttpGet]
    public async Task<ListaModelliTrasmResponse> listaModelliTrasm(bool fascicoli, [FromHeader(Name = "AuthToken")] string token)
    {
        ListaModelliTrasmResponse retval = await MobileManager.getListaModelliTrasm(token, fascicoli);
        return retval;
    }

    [Route("EseguiTrasmissioneModello")]
    //[ResponseType(typeof(EseguiTrasmResponse))]
    [HttpPost]
    public async Task<EseguiTrasmResponse> eseguiTrasmModello(EseguiTrasmRequest request, [FromHeader(Name = "AuthToken")] string token)
    {
        EseguiTrasmResponse retval = await MobileManager.eseguiTrasm(token, request);
        return retval;
    }

    [Route("EseguiTrasmissione")]
    //[ResponseType(typeof(EseguiTrasmResponse))]
    [HttpPost]
    public async Task<EseguiTrasmResponse> eseguiTrasmDiretta(EseguiTrasmDirettaRequest request, [FromHeader(Name = "AuthToken")] string token)
    {
        EseguiTrasmResponse retval = await MobileManager.eseguiTrasmDiretta(token, request);
        return retval;
    }

    [Route("ListaCorrispondenti")]
    //[ResponseType(typeof(RicercaSmistamentoResponse))]
    [HttpPost]
    public async Task<RicercaSmistamentoResponse> ListaCorrVeloce(GetListaCorrVeloceRequest request, [FromHeader(Name = "AuthToken")] string token)
    {
        RicercaSmistamentoResponse retval = await MobileManager.GetListaCorrispondentiVeloce(token, request);
        return retval;
    }

    [Route("ListaRuoliUtente")]
    //[ResponseType(typeof(ListaRuoliByUserResponse))]
    [HttpGet]
    public async Task<ListaRuoliByUserResponse> getListaRuoli(string idUtente, [FromHeader(Name = "AuthToken")] string token)
    {
        ListaRuoliByUserResponse retval = await MobileManager.getListaRuoliUser(token, idUtente);
        return retval;
    }

    [Route("ListaPreferiti")]
    //[ResponseType(typeof(ListaPreferitiResponse))]
    [HttpGet]
    public async Task<ListaPreferitiResponse> ListaPreferiti([FromHeader(Name = "AuthToken")] string token, string soloPers = "false", string tipoPref = "A")
    {
        ListaPreferitiResponse retval = await MobileManager.PrefMobile_getList(token, soloPers, tipoPref);
        return retval;
    }

    [Route("Preferito")]
    //[ResponseType(typeof(bool))]
    [HttpPut]
    public async Task<bool> NuovoPreferito(DocsPaVO.Mobile.InfoPreferito infoPref, [FromHeader(Name = "AuthToken")] string token)
    {
        bool retval = await MobileManager.PrefMobile_insert(token, infoPref);
        return retval;
    }

    [Route("Preferito")]
    //[ResponseType(typeof(bool))]
    [HttpDelete]
    public async Task<bool> RimuoviPreferito(DocsPaVO.Mobile.InfoPreferito infoPref, [FromHeader(Name = "AuthToken")] string token)
    {
        bool retval = await MobileManager.PrefMobile_delete(token, infoPref);
        return retval;
    }

    [Route("ListaRagioni")]
    //[ResponseType(typeof(ListaRagioniResponse))]
    [HttpGet]
    public async Task<ListaRagioniResponse> ListaRagioni(string idObject, string docFasc, [FromHeader(Name = "AuthToken")] string token)
    {
        ListaRagioniResponse retval = await MobileManager.getListaRagioni(token, idObject, docFasc);
        return retval;
    }

}
