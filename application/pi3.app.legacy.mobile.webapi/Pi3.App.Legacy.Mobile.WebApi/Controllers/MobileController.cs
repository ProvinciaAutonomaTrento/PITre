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
using System.Net;
using DocsPaVO.Mobile.Responses;
using Pi3.App.Legacy.Mobile.WebApi.Model;
using Pi3.App.Legacy.Mobile.WebApi.Manager;

namespace Pi3.App.Legacy.Mobile.WebApi.Controllers;


//[Authorize(policy: Policies.PITRE)]
//[Authorize()]
[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
public class MobileController(
    ILogger<MobileController> logger ) : ControllerBase
{
    readonly ILogger<MobileController> _logger = logger;


    [Route("CheckConnection")]
//    [ResponseType(typeof(string))]
    [HttpGet]
    //[DocsPaActionFilter]
    public async Task<string> CheckConn()
    {
        bool testX = BusinessLogic.Amministrazione.UtenteManager.Checkconnection();
        string retVal = "";
        if (testX) retVal = "OK";
        else retVal = "ERROR";

        return retVal;
    }

    [Route("GetTipiOggetto")]
    //[ResponseType(typeof(IList<string>))]
    [HttpGet]
    public async Task<IList<string>> GetTipiOggetto()
    {
        IList<string> tipiOgg = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getTipiOggetto().Cast<string>().ToList<string>();

        return tipiOgg;
    }

    [Route("ListaIstanze")]
    //[ResponseType(typeof(IList<DocsPaVO.Mobile.Istanza>))]
    [HttpGet]
    public async Task<IList<DocsPaVO.Mobile.Istanza>> ListaIstanze()
    {
        IList<DocsPaVO.Mobile.Istanza> retval = null;
        DocsPaVO.Mobile.Istanza tempX = new DocsPaVO.Mobile.Istanza();
        System.Collections.ArrayList istanze = new System.Collections.ArrayList();
        try
        {
            istanze = await MobileManager.ListaIstanze();
            logger.LogDebug(istanze == null ? "istanze null" : "Istanze not null " + istanze.Count);
        }
        catch (Exception extask)
        {
            logger.LogError(extask, null, null);
            istanze = null;
        }
        if (istanze == null)
        {
            istanze = new System.Collections.ArrayList();
            // Sviluppo e Collaudo
            tempX = new DocsPaVO.Mobile.Istanza("Sviluppo", "Ambiente di sviluppo pc", "Sviluppo", "http://192.168.196.246/Vt-Docs/DocsPaRESTWS/");
            istanze.Add(tempX);
            tempX = new DocsPaVO.Mobile.Istanza("Sviluppo", "Ambiente di sviluppo mod ", "Sviluppo", "http://172.17.169.209/DocsPaRESTWSTN/");
            istanze.Add(tempX);
            tempX = new DocsPaVO.Mobile.Istanza("PAT_TEST - Collaudo", "Provincia Autonoma di Trento - Ambiente di collaudo", "Collaudo", "https://mobile-test.pitre.tn.it/PAT-be/");
            istanze.Add(tempX);
            // Produzione
            tempX = new DocsPaVO.Mobile.Istanza("APSS", "Azienda Provinciale per i Servizi Sanitari di Trento", "Produzione", "Da definire");
            istanze.Add(tempX);
            tempX = new DocsPaVO.Mobile.Istanza("COMPRENSORI", "Istanza delle comunita'", "Produzione", "Da definire");
            istanze.Add(tempX);
            tempX = new DocsPaVO.Mobile.Istanza("COMTN", "Comune di Trento", "Produzione", "Da definire");
            istanze.Add(tempX);
            tempX = new DocsPaVO.Mobile.Istanza("COMUNI", "Istanza dei comuni", "Produzione", "Da definire");
            istanze.Add(tempX);
            tempX = new DocsPaVO.Mobile.Istanza("CONS_COMUNI", "Consorzio dei Comuni Trentini - Consiglio delle Autonomie Locali", "Produzione", "Da definire");
            istanze.Add(tempX);
            tempX = new DocsPaVO.Mobile.Istanza("ENTI", "Istanza degli enti", "Produzione", "Da definire");
            istanze.Add(tempX);
            tempX = new DocsPaVO.Mobile.Istanza("INFOTN", "Informatica Trentina spa", "Produzione", "Da definire");
            istanze.Add(tempX);
            tempX = new DocsPaVO.Mobile.Istanza("PAT", "Provincia Autonoma di Trento", "Produzione", "Da definire");
            istanze.Add(tempX);
            tempX = new DocsPaVO.Mobile.Istanza("REGIONE", "Regione Autonoma Trentino Alto Adige/Sudtirol", "Produzione", "Da definire");
            istanze.Add(tempX);
            tempX = new DocsPaVO.Mobile.Istanza("SCUOLE", "Istanza delle scuole", "Produzione", "Da definire");
            istanze.Add(tempX);
            tempX = new DocsPaVO.Mobile.Istanza("UNITN", "Universita' degli Studi di Trento", "Produzione", "Da definire");
            istanze.Add(tempX);

            // Test
            tempX = new DocsPaVO.Mobile.Istanza("APSS_Test", "Azienda Provinciale per i Servizi Sanitari di Trento - TEST", "Test", "Da definire");
            istanze.Add(tempX);
            tempX = new DocsPaVO.Mobile.Istanza("COMPRENSORI_Test", "Istanza di comunità - TEST", "Test", "Da definire");
            istanze.Add(tempX);
            tempX = new DocsPaVO.Mobile.Istanza("COMTN_Test", "Comune di Trento - TEST", "Test", "Da definire");
            istanze.Add(tempX);
            tempX = new DocsPaVO.Mobile.Istanza("COMUNI_Test", "Istanza dei comuni - TEST", "Test", "Da definire");
            istanze.Add(tempX);
            tempX = new DocsPaVO.Mobile.Istanza("CONS_COMUNI_Test", "Consorzio dei Comuni Trentini - Consiglio delle Autonomie Locali - TEST", "Test", "Da definire");
            istanze.Add(tempX);
            tempX = new DocsPaVO.Mobile.Istanza("ENTI_Test", "Istanza degli enti - TEST", "Test", "Da definire");
            istanze.Add(tempX);
            tempX = new DocsPaVO.Mobile.Istanza("INFOTN_Test", "Informatica Trentina spa - TEST", "Test", "Da definire");
            istanze.Add(tempX);
            tempX = new DocsPaVO.Mobile.Istanza("PAT_Test", "Provincia Autonoma di Trento - TEST", "Test", "Da definire");
            istanze.Add(tempX);
            tempX = new DocsPaVO.Mobile.Istanza("REGIONE_Test", "Regione Autonoma Trentino Alto Adige/Sudtirol - TEST", "Test", "Da definire");
            istanze.Add(tempX);
            tempX = new DocsPaVO.Mobile.Istanza("SCUOLE_Test", "Istanza delle scuole - TEST", "Test", "Da definire");
            istanze.Add(tempX);
            tempX = new DocsPaVO.Mobile.Istanza("UNITN_Test", "Universita' degli Studi di Trento - TEST", "Test", "Da definire");
            istanze.Add(tempX);

            // Quality
            tempX = new DocsPaVO.Mobile.Istanza("APSS_Quality", "Azienda Provinciale per i Servizi Sanitari di Trento - QUALITY", "Quality", "Da definire");
            istanze.Add(tempX);
            tempX = new DocsPaVO.Mobile.Istanza("COMPRENSORI_Quality", "Istanza di comunità - QUALITY", "Quality", "Da definire");
            istanze.Add(tempX);
            tempX = new DocsPaVO.Mobile.Istanza("COMTN_Quality", "Comune di Trento - QUALITY", "Quality", "Da definire");
            istanze.Add(tempX);
            tempX = new DocsPaVO.Mobile.Istanza("COMUNI_Quality", "Istanza dei comuni - QUALITY", "Quality", "Da definire");
            istanze.Add(tempX);
            tempX = new DocsPaVO.Mobile.Istanza("CONS_COMUNI_Quality", "Consorzio dei Comuni Trentini - Consiglio delle Autonomie Locali - QUALITY", "Quality", "Da definire");
            istanze.Add(tempX);
            tempX = new DocsPaVO.Mobile.Istanza("ENTI_Quality", "Istanza degli enti - QUALITY", "Quality", "Da definire");
            istanze.Add(tempX);
            tempX = new DocsPaVO.Mobile.Istanza("INFOTN_Quality", "Informatica Trentina spa - QUALITY", "Quality", "Da definire");
            istanze.Add(tempX);
            tempX = new DocsPaVO.Mobile.Istanza("PAT_Quality", "Provincia Autonoma di Trento - QUALITY", "Quality", "Da definire");
            istanze.Add(tempX);
            tempX = new DocsPaVO.Mobile.Istanza("REGIONE_Quality", "Regione Autonoma Trentino Alto Adige/Sudtirol - QUALITY", "Quality", "Da definire");
            istanze.Add(tempX);
            tempX = new DocsPaVO.Mobile.Istanza("SCUOLE_Quality", "Istanza delle scuole - QUALITY", "Quality", "Da definire");
            istanze.Add(tempX);
            tempX = new DocsPaVO.Mobile.Istanza("UNITN_Quality", "Universita' degli Studi di Trento - QUALITY", "Quality", "Da definire");
            istanze.Add(tempX);
        }

        retval = istanze.Cast<DocsPaVO.Mobile.Istanza>().ToList<DocsPaVO.Mobile.Istanza>();
        return retval;
    }

    [Route("Login")]
    //[ResponseType(typeof(LoginResponse))]
    [HttpPost]
    public async Task<LoginResponse> Authenticate(AuthenticateRequest request)
    {
        LoginResponse retval;

        if (!string.IsNullOrWhiteSpace(request.Otp))
            retval = await MobileManager.resetPassword(request);
        else

         if (string.IsNullOrWhiteSpace(request.OldPassword))
            retval = await MobileManager.login(request);
        else
            retval = await MobileManager.cambioPassword(request);

        return retval;
    }

    [Route("RecuperaPasswordOTP")]
    //[ResponseType(typeof(LoginResponse))]
    [HttpPost]
    public async Task<LoginResponse> RecuperaPasswordOTP(AuthenticateRequest request)
    {
        LoginResponse retval = await MobileManager.resetPasswordInviaOTP(request);

        return retval;
    }

    [Route("Update")]
    //[ResponseType(typeof(VerifyUpdateResponse))]
    [HttpPost]
    public async Task<VerifyUpdateResponse> VerifyUpdate(VerifyUpdateRequest request)
    {
        VerifyUpdateResponse retval = await MobileManager.verifyUpdate(request);

        return retval;
    }

    [Route("EsercitaDelega")]
    //[ResponseType(typeof(LoginResponse))]
    [HttpPost]
    public async Task<LoginResponse> EsercitaDelega(DocsPaVO.Mobile.Delega delega,
        [FromHeader(Name = "AuthToken")] string token)
    {
        LoginResponse retval = await MobileManager.esercitaDelega(token, delega.Id, delega.IdRuoloDelegante, delega.InfoDelega.id_utente_delegante);

        return retval;
    }

    [Route("TerminaDelega")]
    //[ResponseType(typeof(bool))]
    [HttpPost]
    public async Task<bool> revocaDelega([FromHeader(Name = "AuthToken")] string token)
    {
        bool retval = await MobileManager.terminaDelega(token);
        return retval;
    }

    [Route("ListaAmmByUser")]
    //[ResponseType(typeof(DocsPaVO.utente.Amministrazione[]))]
    [HttpGet]
    public async Task<DocsPaVO.utente.Amministrazione[]> getListaAmmByUser(string userid)
    {
        DocsPaVO.utente.Amministrazione[] retval = await MobileManager.getListaAmmByUser(userid);
        return retval;
    }

    [Route("LogOut")]
    //[ResponseType(typeof(LogoutResponse))]
    [HttpPost]
    public async Task<LogoutResponse> LogOut(string dst, [FromHeader(Name = "AuthToken")] string token)
    {
        LogoutResponse retval = await MobileManager.logout(token, dst);
        return retval;
    }

    [Route("DelegaCheckAttiva")]
    //[ResponseType(typeof(int))]
    [HttpGet("DelegaCheckAttiva")]
    public async Task<int> DelegaCheckAttiva([FromHeader(Name = "AuthToken")] string token)
    {
        int retval = await MobileManager.DelegaCheckAttiva(token);
        return retval;
    }

    [Route("TodoList")]
    //[ResponseType(typeof(ToDoListResponse))]
    [HttpGet]
    public async Task<ToDoListResponse> getTodoList([FromHeader(Name = "AuthToken")] string token, int pagesize = 100, int requestedpage = 1)
    {
        /*TodoListRequest request*/
        //string token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
        //logger.Debug("getTodolist controller - Start");
        //logger.Debug("request null ? " + request == null ? "si" : "no");
        //logger.Debug("token" + token);
        //logger.DebugFormat("Request valori {0} {1}", request.PageSize, request.RequestedPage);
        //ToDoListResponse retval = await MobileManager.getTodoList(token, request.RequestedPage, request.PageSize);
        ToDoListResponse retval = await MobileManager.getTodoList(token, requestedpage, pagesize);

        return retval;
    }

    [Route("CambioRuolo")]
    //[ResponseType(typeof(LoginResponse))]
    [HttpPost]
    public async Task<LoginResponse> cambiaruolo(string idRuolo, [FromHeader(Name = "AuthToken")] string token)
    {
        LoginResponse retval = await MobileManager.cambiaRuolo(token, idRuolo);
        return retval;
    }


}
